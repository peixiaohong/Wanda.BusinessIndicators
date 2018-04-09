
jQuery.fn.CloneTableHeader = function (tableId, tableParentDivId) {

    //获取冻结表头所在的DIV,如果DIV已存在则移除
    var obj = document.getElementById("tableHeaderDiv" + tableId);
    if (obj) {
        jQuery(obj).remove();
    }

    var browserName = navigator.appName;//获取浏览器信息,用于后面代码区分浏览器
    var ver = navigator.appVersion;
    var browserVersion = parseFloat(ver.substring(ver.indexOf("MSIE") + 5, ver.lastIndexOf("Windows")));
    var scrollWidth = 0;
    var content = document.getElementById(tableParentDivId);
    if (content)
    {
        scrollWidth=content.offsetWidth - content.clientWidth;
    }

    var tableOrg = jQuery("#" + tableId);//获取表内容
    var table = tableOrg.clone();//克隆表内容
    table.attr("id", "cloneTable");

    //注意:需要将要冻结的表头放入thead中 
    var tableHeader = jQuery(tableOrg).find("thead");
    var tableHeaderHeight = tableHeader.height();
    tableHeader.hide();
    var colsWidths = jQuery(tableOrg).find("tbody tr:first td").map(function () {
        return jQuery(this).width();
    });//动态获取每一列的宽度

    var tableCloneCols = jQuery(table).find("thead tr:first td")
    if (colsWidths.size() > 0) {//根据浏览器为冻结的表头宽度赋值(主要是区分IE8)
        for (i = 0; i < tableCloneCols.size() ; i++) {
            if (i == tableCloneCols.size() - 1) {
                if (browserVersion == 8.0)
                    tableCloneCols.eq(i).width(colsWidths[i] + scrollWidth);
                else
                    tableCloneCols.eq(i).width(colsWidths[i]);
            } else {
                tableCloneCols.eq(i).width(colsWidths[i]);
            }
        }
    }

    //创建冻结表头的DIV容器,并设置属性

    var headerDiv = document.createElement("div");
    headerDiv.appendChild(table[0]);
    jQuery(headerDiv).css("height", tableHeaderHeight);
    jQuery(headerDiv).css("overflow", "hidden");
    jQuery(headerDiv).css("z-index", "20");
    jQuery(headerDiv).css("width", "100%");
    jQuery(headerDiv).attr("id", "tableHeaderDiv" + tableId);
    jQuery(headerDiv).insertBefore(tableOrg.parent());

}