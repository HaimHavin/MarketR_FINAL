var tableToExcel = (function () {
    var uri = 'data:application/vnd.ms-excel;base64,'
      , template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" ><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><meta http-equiv="content-type" content="application/vnd.ms-excel; charset=UTF-8"><meta charset="UTF-8"><body><table>{table}</table></body></html>'
      , base64 = function (s) { return window.btoa(unescape(encodeURIComponent(s))) }
      , format = function (s, c) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }) }
    return function (table, name) {
        var tablHtml = "";
        if (!table.nodeType) tablHtml = document.getElementById(table)
        //var ctx = { worksheet: name || 'Worksheet', table: table.innerHTML }
        debugger;
        var data = '';
        if (tablHtml.rows.length > 1) {
            $.each(tablHtml.rows, function (index, item) {
                if ($(this).find('th').text() != null && $(this).find('th').text().match('LIQUIDATE') != null) {
                    $(this).find('th').last().remove();
                };
                if ($(this).find('td').children() != undefined && $(this).find('td').children()[0] != undefined && $(this).find('td').children()[0].className == 'checkbox') {
                    $(this).find('td').last().remove();
                }
            });            
            var ctx = { worksheet: name || 'Worksheet', table: tablHtml.innerHTML }
            window.location.href = uri + base64(format(template, ctx));
            $("#remoteModal").modal('hide');
        }
    }
})();