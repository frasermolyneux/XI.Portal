using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNet.Identity;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.Auth;
using XI.Portal.Library.Auth.Extensions;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Library.DemoManager;
using XI.Portal.Library.DemoManager.Extensions;
using XI.Portal.Library.DemoManager.Models;
using XI.Portal.Library.Logging;
using XI.Portal.Web.Portal.ViewModels.Demos;

namespace XI.Portal.Web.Portal.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.LoggedInUser)]
    public class DemosController : Controller
    {
        private readonly ApplicationUserManager applicationUserManager;
        private readonly IContextProvider contextProvider;
        private readonly IDatabaseLogger databaseLogger;
        private readonly DemoRepositoryConfig demoRepositoryConfig;

        public DemosController(IContextProvider contextProvider, ApplicationUserManager applicationUserManager,
            DemoRepositoryConfig demoRepositoryConfig, IDatabaseLogger databaseLogger)
        {
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            this.applicationUserManager = applicationUserManager ??
                                          throw new ArgumentNullException(nameof(applicationUserManager));
            this.demoRepositoryConfig =
                demoRepositoryConfig ?? throw new ArgumentNullException(nameof(demoRepositoryConfig));
            this.databaseLogger = databaseLogger ?? throw new ArgumentNullException(nameof(databaseLogger));
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var user = await applicationUserManager.FindByNameAsync(User.Identity.Name);

            using (var context = contextProvider.GetContext())
            {
                var demoIndexViewModel = new DemoIndexViewModel
                {
                    DemoManagerAuthKey = user.DemoManagerAuthKey
                };

                if (User.Identity.IsInAdminOrModeratorRole())
                    demoIndexViewModel.Demos = await context.Demos.Include(demo => demo.User).ToListAsync();
                else
                    demoIndexViewModel.Demos = await context.Demos.Include(demo => demo.User)
                        .Where(demo => demo.User == user).ToListAsync();

                return View(demoIndexViewModel);
            }
        }

        [HttpPost]
        public async Task<ActionResult> GenerateDemoManagerAuthKey()
        {
            var user = await applicationUserManager.FindByNameAsync(User.Identity.Name);
            user.DemoManagerAuthKey = Guid.NewGuid().ToString();
            await applicationUserManager.UpdateAsync(user);
            await databaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                "User has regenerated their demo manager auth key");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult UploadDemo()
        {
            return View(new UploadDemoViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> UploadDemo(UploadDemoViewModel model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (file.ContentLength == 0)
            {
                ModelState.AddModelError("file", "You must provide a demo file");
                return View(model);
            }

            var whitelistedExtensions = new List<string> {".dm_1", ".dm_6"};

            if (!whitelistedExtensions.Any(ext => file.FileName.EndsWith(ext)))
            {
                ModelState.AddModelError("file", "Invalid file type - this must be a demo file");
                return View(model);
            }

            var fileName = $"{Guid.NewGuid().ToString()}.{model.Game.DemoExtension()}";
            var path = Path.Combine(Path.GetTempPath(), fileName);
            file.SaveAs(path);

            SaveToS3(path, model.Game);

            var localDemo = new LocalDemo(path, model.Game);

            using (var context = contextProvider.GetContext())
            {
                var user = await context.Users.SingleOrDefaultAsync(u => u.UserName == User.Identity.Name);

                var demo = new Demo
                {
                    Game = model.Game,
                    Name = file.FileName,
                    FileName = fileName,
                    Date = localDemo.Date,
                    Map = localDemo.Map,
                    Mod = localDemo.Mod,
                    GameType = localDemo.GameType,
                    Server = localDemo.Server,
                    Size = localDemo.Size,
                    User = user
                };

                context.Demos.Add(demo);
                await context.SaveChangesAsync();
                await databaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                    $"User has uploaded a new demo: {demo.DemoId}");

                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ClientDemoList()
        {
            if (Request.Headers.GetValues("demo-manager-auth-key") == null ||
                Request.Headers.GetValues("demo-manager-auth-key")?.Length == 0)
                return Content("AuthError: No auth key provided in the request");

            var header = Request.Headers.GetValues("demo-manager-auth-key");
            var authKey = header?.First();

            if (string.IsNullOrWhiteSpace(authKey))
            {
                await databaseLogger.CreateSystemLogAsync("Security", $"ClientDemoList - Auth key header supplied but invalid - {authKey}");
                return Content("AuthError: Auth key header supplied but invalid");
            }

            using (var context = contextProvider.GetContext())
            {
                var user = await context.Users.SingleOrDefaultAsync(u => u.DemoManagerAuthKey == authKey);

                if (user == null)
                {
                    await databaseLogger.CreateSystemLogAsync("Security", $"ClientDemoList - Auth key supplied but invalid - {authKey}");
                    return Content("AuthError: Auth key supplied but invalid. Try re-entering the auth key on your client");
                }

                if (XtremeIdiotsRolesHelper.IsGroupIdAdminOrModerator(Convert.ToInt32(user.XtremeIdiotsPrimaryGroupId)))
                {
                    var demos = await context.Demos.Select(demo => new
                    {
                        demo.DemoId,
                        Version = demo.Game,
                        demo.Name,
                        demo.Date,
                        demo.Map,
                        demo.Mod,
                        demo.GameType,
                        demo.Server,
                        demo.Size,
                        Identifier = demo.FileName,
                        demo.FileName
                    }).ToListAsync();

                    return Json(demos, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var demos = await context.Demos.Where(demo => demo.User == user).Select(demo => new
                    {
                        demo.DemoId,
                        Version = demo.Game,
                        demo.Name,
                        demo.Date,
                        demo.Map,
                        demo.Mod,
                        demo.GameType,
                        demo.Server,
                        demo.Size,
                        Identifier = demo.FileName,
                        demo.FileName
                    }).ToListAsync();

                    return Json(demos, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ClientUploadDemo(HttpPostedFileBase file)
        {
            if (Request.Headers.GetValues("demo-manager-auth-key") == null ||
                Request.Headers.GetValues("demo-manager-auth-key")?.Length == 0)
                return Content("AuthError: No auth key provided in the request");

            var header = Request.Headers.GetValues("demo-manager-auth-key");
            var authKey = header?.First();

            if (string.IsNullOrWhiteSpace(authKey))
            {
                await databaseLogger.CreateSystemLogAsync("Security", $"ClientUploadDemo - Auth key header supplied but invalid - {authKey}");
                return Content("AuthError: Auth key header supplied but invalid");
            }

            using (var context = contextProvider.GetContext())
            {
                var user = await context.Users.SingleOrDefaultAsync(u => u.DemoManagerAuthKey == authKey);

                if (user == null)
                {
                    await databaseLogger.CreateSystemLogAsync("Security", $"ClientUploadDemo - Auth key supplied but invalid - {authKey}");
                    return Content("AuthError: Auth key supplied but invalid. Try re-entering the auth key on your client");
                }

                if (file.ContentLength == 0) return Content("You must provide a file to be uploaded");

                var whitelistedExtensions = new List<string> {".dm_1", ".dm_6"};

                if (!whitelistedExtensions.Any(ext => file.FileName.EndsWith(ext)))
                    return Content("Invalid file type - this must be a demo file");

                var gameTypeHeader = Request.Headers.GetValues("demo-manager-game-type");
                Enum.TryParse(gameTypeHeader?.First(), out GameType gameType);

                var fileName = $"{Guid.NewGuid().ToString()}.{gameType.DemoExtension()}";
                var path = Path.Combine(Path.GetTempPath(), fileName);
                file.SaveAs(path);

                SaveToS3(path, gameType);

                var localDemo = new LocalDemo(path, gameType);

                var demo = new Demo
                {
                    Game = gameType,
                    Name = file.FileName,
                    FileName = fileName,
                    Date = localDemo.Date,
                    Map = localDemo.Map,
                    Mod = localDemo.Mod,
                    GameType = localDemo.GameType,
                    Server = localDemo.Server,
                    Size = localDemo.Size,
                    User = user
                };

                context.Demos.Add(demo);
                await context.SaveChangesAsync();
                await databaseLogger.CreateUserLogAsync(user.Id,
                    $"User has uploaded a new demo through the client: {demo.DemoId}");

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Download(string id)
        {
            if (!Guid.TryParse(id, out var idAsGuid))
                return RedirectToAction("Index");

            using (var context = contextProvider.GetContext())
            {
                var demo = await context.Demos.SingleOrDefaultAsync(d => d.DemoId == idAsGuid);

                if (demo == null) return RedirectToAction("Index");

                using (var client = new WebClient())
                {
                    var uri = $"https://s3.us-east-2.amazonaws.com/demomanager-prd/demos/{demo.Game}/{demo.FileName}";
                    var file = client.DownloadData(uri);

                    var cd = new ContentDisposition
                    {
                        FileName = demo.Name,
                        Inline = false
                    };
                    Response.AppendHeader("Content-Disposition", cd.ToString());

                    return File(file, cd.DispositionType);
                }
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> ClientDownload(string id)
        {
            if (!Guid.TryParse(id, out var idAsGuid))
                return RedirectToAction("Index");

            using (var context = contextProvider.GetContext())
            {
                var demo = await context.Demos.SingleOrDefaultAsync(d => d.DemoId == idAsGuid);

                if (demo == null) return RedirectToAction("Index");

                using (var client = new WebClient())
                {
                    var uri = $"https://s3.us-east-2.amazonaws.com/demomanager-prd/demos/{demo.Game}/{demo.FileName}";
                    var file = client.DownloadData(uri);

                    var cd = new ContentDisposition
                    {
                        FileName = demo.Name,
                        Inline = false
                    };
                    Response.AppendHeader("Content-Disposition", cd.ToString());

                    return File(file, cd.DispositionType);
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            if (!Guid.TryParse(id, out var idAsGuid))
                return RedirectToAction("Index");

            using (var context = contextProvider.GetContext())
            {
                var demo = await context.Demos.SingleOrDefaultAsync(d => d.DemoId == idAsGuid);

                if (demo == null)
                    return RedirectToAction("Index");

                if (!User.Identity.IsInSeniorAdminRole() && User.Identity.GetUserId() != demo.User.Id)
                {
                    await databaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                        $"User attempted to delete a demo they do not have permissions to {demo.DemoId}");
                    return RedirectToAction("Index");
                }

                return View(demo);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (!Guid.TryParse(id, out var idAsGuid))
                return RedirectToAction("Index");

            using (var context = contextProvider.GetContext())
            {
                var demo = await context.Demos.SingleOrDefaultAsync(d => d.DemoId == idAsGuid);

                if (demo == null)
                    return RedirectToAction("Index");

                if (!User.Identity.IsInSeniorAdminRole() && User.Identity.GetUserId() != demo.User.Id)
                    return RedirectToAction("Index");

                context.Demos.Remove(demo);
                await context.SaveChangesAsync();
                await databaseLogger.CreateUserLogAsync(User.Identity.GetUserId(), $"User has deleted a demo: {id}");

                return RedirectToAction("Index");
            }
        }

        private void SaveToS3(string filePath, GameType gameType)
        {
            var s3BucketName = "demomanager-prd";
            var client = new AmazonS3Client(new BasicAWSCredentials("AKIAJ4TMXDNLOC4DA7JQ", "Y48IbyCQcJo84g9EFWl9wmRXo4X3LKDTC4+BHadk"));

            var fileInfo = new FileInfo(filePath);
            var key = $"demos/{gameType}/{fileInfo.Name}";
            using (var stream = new StreamReader(fileInfo.FullName))
            {
                var s3Request = new PutObjectRequest
                {
                    AutoCloseStream = false,
                    BucketName = s3BucketName,
                    InputStream = stream.BaseStream,
                    Key = key
                };

                client.PutObject(s3Request);
            }
        }
    }
}