using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Razor;
using System.Web.Razor.Parser;
using System.Web.Razor.Text;

namespace TestApplication.Test
{
    public class TestTestClass
    {
         private const string Text =
@"
/*!
** Unobtrusive Ajax support library for jQuery
** Copyright (C) Microsoft Corporation. All rights reserved.
*/

/*jslint white: true, browser: true, onevar: true, undef: true, nomen: true, eqeqeq: true, plusplus: true, bitwise: true, regexp: true, newcap: true, immed: true, strict: false */
/*global window: false, jQuery: false */

(function ($) {
    var data_click = 'unobtrusiveAjaxClick',
        data_validation = 'unobtrusiveValidation';

    function getFunction(code, argNames) {
        var fn = window, parts = (code || '').split('.');
        while (fn && parts.length) {
            fn = fn[parts.shift()];
        }
@if(x > 5)
{
    <Text>
        aaaaa
    </Text>
}
 if (typeof (fn) === 'function') {
            return fn;
        }
        argNames.push(code);
        return Function.constructor.apply(null, argNames);
    }

    function isMethodProxySafe(method) {
        return method === 'GET' || method === 'POST';
    }

    function asyncOnBeforeSend(xhr, method) {
        if (!isMethodProxySafe(method)) {
            xhr.setRequestHeader('X-HTTP-Method-Override', method);
        }
    }

    function asyncOnSuccess(element, data, contentType) {
        var mode;

        if (contentType.indexOf('application/x-javascript') !== -1) {  // jQuery already executes JavaScript for us
            return;
        }

        mode = (element.getAttribute('data-ajax-mode') || '').toUpperCase();
        $(element.getAttribute('data-ajax-update')).each(function (i, update) {
            var top;

            switch (mode) {
            case 'BEFORE':
                top = update.firstChild;
                $('<div />').html(data).contents().each(function () {
                    update.insertBefore(this, top);
                });
                break;
            case 'AFTER':
                $('<div />').html(data).contents().each(function () {
                    update.appendChild(this);
                });
                break;
            default:
                $(update).html(data);
                break;
            }
        });
   
    });
}(jQuery));
";

        public static void Parse()
        {
            var textReader = new StringReader(Text);

            HtmlMarkupParser htmlParser = new HtmlMarkupParser();
            CSharpCodeParser csharpParser = new CSharpCodeParser();
            ParserContext parserContext = new ParserContext(new SeekableTextReader(textReader), csharpParser, htmlParser, htmlParser)
            {
                DesignTimeMode = true
            };
            htmlParser.Context = parserContext;
            csharpParser.Context = parserContext;
            htmlParser.ParseDocument();
            ParserResults parserResults = parserContext.CompleteParse();
        }
    }

}