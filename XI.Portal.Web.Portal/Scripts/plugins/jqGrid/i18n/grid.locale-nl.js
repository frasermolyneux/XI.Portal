/**
 * jqGrid Dutch Translation
**/

/*jslint white: true */
/*global jQuery, module, require */
(function(factory) {
    "use strict";
    if (typeof define === "function" && define.amd) {
        // AMD. Register as an anonymous module.
        define(["jquery"], factory);
    } else if (typeof module === "object" && module.exports) {
        // Node/CommonJS
        module.exports = function(root, $) {
            if ($ === undefined) {
                // require("jquery") returns a factory that requires window to
                // build a jQuery instance, we normalize how we use modules
                // that require this pattern but the window provided is a noop
                // if it's defined (how jquery works)
                $ = typeof window !== "undefined" ? require("jquery") : require("jquery")(root || window);
            }
            factory($);
            return $;
        };
    } else {
        // Browser globals
        factory(jQuery);
    }
}(function($) {
    "use strict";
    var locInfo = {
        isRTL: false,
        defaults: {
            recordtext: "regels {0} - {1} van {2}",
            emptyrecords: "Geen data gevonden.",
            loadtext: "Laden...",
            pgtext: "pagina  {0}  van {1}",
            pgfirst: "Eerste Pagina",
            pglast: "Laatste Pagina",
            pgnext: "Volgende Pagina",
            pgprev: "Vorige Pagina",
            pgrecs: "Records per Pagina",
            showhide: "Schakelen Uitklappen Inklappen Grid",
            savetext: "Opslaan..."
        },
        search: {
            caption: "Zoeken...",
            Find: "Zoek",
            Reset: "Herstellen",
            odata: [
                { oper: "eq", text: "gelijk aan" },
                { oper: "ne", text: "niet gelijk aan" },
                { oper: "lt", text: "kleiner dan" },
                { oper: "le", text: "kleiner dan of gelijk aan" },
                { oper: "gt", text: "groter dan" },
                { oper: "ge", text: "groter dan of gelijk aan" },
                { oper: "bw", text: "begint met" },
                { oper: "bn", text: "begint niet met" },
                { oper: "in", text: "is in" },
                { oper: "ni", text: "is niet in" },
                { oper: "ew", text: "eindigt met" },
                { oper: "en", text: "eindigt niet met" },
                { oper: "cn", text: "bevat" },
                { oper: "nc", text: "bevat niet" },
                { oper: "nu", text: "is null" },
                { oper: "nn", text: "is not null" }
            ],
            groupOps: [
                { op: "AND", text: "alle" },
                { op: "OR", text: "een van de" }
            ],
            addGroupTitle: "Add subgroup",
            deleteGroupTitle: "Delete group",
            addRuleTitle: "Add rule",
            deleteRuleTitle: "Delete rule",
            operandTitle: "Klik om de zoekterm te selecteren.",
            resetTitle: "Herstel zoekterm"
        },
        edit: {
            addCaption: "Nieuw",
            editCaption: "Bewerken",
            bSubmit: "Opslaan",
            bCancel: "Annuleren",
            bClose: "Sluiten",
            saveData: "Er is data aangepast! Wijzigingen opslaan?",
            bYes: "Ja",
            bNo: "Nee",
            bExit: "Sluiten",
            msg: {
                required: "Veld is verplicht",
                number: "Voer a.u.b. geldig nummer in",
                minValue: "Waarde moet groter of gelijk zijn aan ",
                maxValue: "Waarde moet kleiner of gelijk zijn aan",
                email: "is geen geldig e-mailadres",
                integer: "Voer a.u.b. een geldig getal in",
                date: "Voer a.u.b. een geldige waarde in",
                url: "is geen geldige URL. Prefix is verplicht ('http://' or 'https://')",
                nodefined: " is niet gedefineerd!",
                novalue: " return waarde is verplicht!",
                customarray: "Aangepaste functie moet array teruggeven!",
                customfcheck: "Aangepaste function moet aanwezig zijn in het geval van aangepaste controle!"
            }
        },
        view: {
            caption: "Tonen",
            bClose: "Sluiten"
        },
        del: {
            caption: "Verwijderen",
            msg: "Verwijder geselecteerde regel(s)?",
            bSubmit: "Verwijderen",
            bCancel: "Annuleren"
        },
        nav: {
            edittext: "",
            edittitle: "Bewerken",
            addtext: "",
            addtitle: "Nieuw",
            deltext: "",
            deltitle: "Verwijderen",
            searchtext: "",
            searchtitle: "Zoeken",
            refreshtext: "",
            refreshtitle: "Vernieuwen",
            alertcap: "Waarschuwing",
            alerttext: "Selecteer a.u.b. een regel",
            viewtext: "",
            viewtitle: "Openen",
            savetext: "",
            savetitle: "Save row",
            canceltext: "",
            canceltitle: "Cancel row editing"
        },
        col: {
            caption: "Tonen/verbergen kolommen",
            bSubmit: "OK",
            bCancel: "Annuleren"
        },
        errors: {
            errcap: "Fout",
            nourl: "Er is geen URL gedefinieerd",
            norecords: "Geen data om te verwerken",
            model: "Lengte van 'colNames' is niet gelijk aan 'colModel'!"
        },
        formatter: {
            integer: {
                thousandsSeparator: ".",
                defaultValue: "0"
            },
            number: {
                decimalSeparator: ",",
                thousandsSeparator: ".",
                decimalPlaces: 2,
                defaultValue: "0.00"
            },
            currency: {
                decimalSeparator: ",",
                thousandsSeparator: ".",
                decimalPlaces: 2,
                prefix: "EUR ",
                suffix: "",
                defaultValue: "0.00"
            },
            date: {
                dayNames: [
                    "Zo", "Ma", "Di", "Wo", "Do", "Vr", "Za", "Zondag",
                    "Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrijdag", "Zaterdag"
                ],
                monthNames: [
                    "Jan", "Feb", "Maa", "Apr", "Mei", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
                    "Januari", "Februari", "Maart", "April", "Mei", "Juni", "Juli", "Augustus", "September", "October",
                    "November", "December"
                ],
                AmPm: ["am", "pm", "AM", "PM"],
                S: function(b) {
                    return b < 11 || b > 13 ? ["st", "nd", "rd", "th"][Math.min((b - 1) % 10, 3)] : "th";
                },
                srcformat: "Y-m-d",
                newformat: "d/m/Y",
                masks: {
                    ShortDate: "n/j/Y",
                    LongDate: "l, F d, Y",
                    FullDateTime: "l d F Y G:i:s",
                    MonthDay: "d F",
                    ShortTime: "G:i",
                    LongTime: "G:i:s",
                    YearMonth: "F, Y"
                }
            }
        }
    };
    $.jgrid = $.jgrid || {};
    $.extend(true,
        $.jgrid,
        {
            defaults: {
                locale: "nl"
            },
            locales: {
                // In general the property name is free, but it's recommended to use the names based on
                // http://www.iana.org/assignments/language-subtag-registry/language-subtag-registry
                // http://rishida.net/utils/subtags/ and RFC 5646. See Appendix A of RFC 5646 for examples.
                // One can use the lang attribute to specify language tags in HTML, and the xml:lang attribute for XML
                // if it exists. See http://www.w3.org/International/articles/language-tags/#extlang
                nl: $.extend({}, locInfo, { name: "Nederlands", nameEnglish: "Dutch" }),
                "nl-NL": $.extend({}, locInfo, { name: "Nederlands (Nederland)", nameEnglish: "Dutch (Netherlands)" })
            }
        });
}));