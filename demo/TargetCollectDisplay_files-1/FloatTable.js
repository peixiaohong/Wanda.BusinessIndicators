
//浮动表头
/*
obj thead的id
tab tbody的id
*/

function sortActualIndex(a, b) {
    return a.ActualIndex - b.ActualIndex;
}

var MissH = true;


var Fixed = false;
var objHead;
var tabBody;
var isFixedHead = true;
var isRpt = "Reported" //默认是上报页面，如果有其它的请修改此处的变量
var _floattable_H_actualindex = new Array();
var _floattable_B_actualindex = new Array();

function FloatHeader(obj, tab, isFixed, _isRpt) {

    if (isFixed != undefined && isFixed == false) {
        isFixedHead = false;
    }

    if (_isRpt != undefined && _isRpt != "Reported") {
        isRpt = _isRpt;
    }

    Fixed = false;
    objHead = $(obj);
    tabBody = $(tab);
    _floattable_H_actualindex = new Array();
    _floattable_B_actualindex = new Array();
    ScrollMethod();
    $(window).scroll(
       function () {
           ScrollMethod();
       }
    );
    $(window).resize(function () {
            ScrollMethod();
    });
}



//做成全局变量，为了好判断
var Alltbar;
var Alltaar

function ScrollMethod() {

    var st = document.documentElement.scrollTop;
    var _hight = 350;

    if (isRpt == "Approve") {
        _hight = 100
        if (!isFixedHead) {
            $(objHead).css("top", st + "px");
        }
    }

    if (st > _hight) {

        if (isRpt == "Reported") {
            if (!isFixedHead) {
                $(objHead).css("top", st + "px");
            }
        } else if(isRpt=="MonthRpt") {
            if (!isFixedHead) {
                $(objHead).css("top", (st - 324) + "px");
            }
        }  

        if (!Fixed) {

            var _floattable_tdCount = 0;
            var counter = 0;
            tabBody.find("tr").each(function (i) {


                if (counter < 10) {
                    if ($(this).css("display") != "none") {
                        $(this).find("td").each(function (c) {
                            if (parseInt($(this).attr("colspan")) == 1 && $(this).css("display") != "none") {
                                InitialData(i, c);
                            }
                        });
                        counter++;
                    }
                }
                
            });

            if (isFixedHead) {
                $(objHead).css("position", "Fixed");
                $(objHead).css("top", "0px");
                MissH = true;
            }
            else {
                $(objHead).css("position", "absolute");
                MissH = false;
            }
            _floattable_B_actualindex.sort(sortActualIndex);
            var tbar = new Array();
            for (var x = 0; x < _floattable_B_actualindex.length; x++) {
                var td = tabBody.find("tr:eq(" + _floattable_B_actualindex[x].rowindex + ")").find("td:eq(" + _floattable_B_actualindex[x].cellindex + ")");
                var wd = _floattable_B_actualindex[x].innerWidth - ($(td).innerWidth() - $(td).width());
                tbar.push({
                    "rowindex": $(td).parent("tr").prevAll().length,
                    "cellindex": $(td).prevAll().length,
                    "actualIndex": _floattable_B_actualindex[x].ActualIndex,
                    "wd": wd
                });
                $(td).css("width", wd + "px");
            }

           



            var taar = new Array();
            var __maxhead = objHead.find("tr").length;
            objHead.find("tr").each(function (i) {
                $(this).find("th").each(function (c) {
                    if ($(this).css("display") != "none") {

                        var thIndex = 0;
                        var actualIndex = GetActualIndex(i, c);

                        for (var ai = 0; ai < _floattable_B_actualindex.length; ai++) {
                            if (_floattable_B_actualindex[ai].ActualIndex == actualIndex) {
                                thIndex = ai;
                            }
                        }
                        taar.push({
                            "rowindex": $(this).parent("tr").prevAll().length,
                            "cellindex": $(this).prevAll().length,
                            "thIndex": thIndex,
                            "actualIndex": actualIndex,
                            "wd": 0
                        });
                        var hwd = 0;
                        var colsc = 0;
                        for (j = 0; j < parseInt($(this).attr("colspan")) ; j++) {
                            if (_floattable_B_actualindex[thIndex]) {
                                colsc += _floattable_B_actualindex[thIndex].colspan;
                                hwd += _floattable_B_actualindex[thIndex].innerWidth;
                                thIndex++;
                            }
                            if (colsc >= parseInt($(this).attr("colspan"))) {
                                break;
                            }
                        }
                        hwd = hwd - ($(this).innerWidth() - $(this).width());
                        $(this).css("width", hwd + "px");
                        taar[taar.length - 1].wd = hwd;
                    }
                });
            });

          


            //alert(WebUtil.jsonToString(tbar));
            //alert(WebUtil.jsonToString(_floattable_B_actualindex));
            //alert(WebUtil.jsonToString(taar));

            
            if (Alltaar == undefined )
            {
                $(objHead).css("width", $(objHead).parent().css("width"));
            }
            Alltaar = taar;
            Alltbar = tbar;

            $(tabBody).css("width", $(tabBody).parent().css("width"));

            Fixed = true;
        }
    }
    else {
        $(objHead).css("position", "static");
        $(objHead).css("top", "0px");
        Fixed = false;
        MissH = true;
        _floattable_H_actualindex = new Array();
        _floattable_B_actualindex = new Array();
    }
}

function GetActualIndex(RowIndex, ColumnIndex) {
    var currenttd = $(objHead).find("tr:eq(" + RowIndex + ")").find("th:eq(" + ColumnIndex + ")");
    if (currenttd.css("display") == "none") return-1;

    if (RowIndex == 0 && ColumnIndex == 0) {
        _floattable_H_actualindex.push({
            "rowindex": RowIndex,
            "cellindex": ColumnIndex,
            "ActualIndex": 0,
            "colspan": parseInt($(currenttd).attr("colspan")),
            "rowspan": parseInt($(currenttd).attr("rowspan"))
        });

        return 0;
    }

    for (var ch = 0; ch < _floattable_H_actualindex.length; ch++) {
        if (_floattable_H_actualindex[ch].rowindex == RowIndex && _floattable_H_actualindex[ch].cellindex == ColumnIndex) {
            return _floattable_H_actualindex[ch].ActualIndex;
        }
    }

    var latestactualIndex = 0;
    for (var ch = 0; ch < _floattable_H_actualindex.length; ch++) {
        if (_floattable_H_actualindex[ch].rowindex == RowIndex) {
            latestactualIndex = _floattable_H_actualindex[ch].ActualIndex + _floattable_H_actualindex[ch].colspan;
        }
    }

    var actualIndex = 0;

    for (var ri = 0; ri < _floattable_H_actualindex.length; ri++) {
        if (_floattable_H_actualindex[ri].rowindex < RowIndex && _floattable_H_actualindex[ri].rowspan>1
            && ((_floattable_H_actualindex[ri].rowindex + _floattable_H_actualindex[ri].rowspan) > RowIndex)
            && _floattable_H_actualindex[ri].ActualIndex <= latestactualIndex) {
            actualIndex = actualIndex + _floattable_H_actualindex[ri].colspan;
            if (latestactualIndex <= actualIndex) {
                latestactualIndex = actualIndex;
            }
        }
    }
    var tr = $(objHead).find("tr:eq(" + RowIndex + ")");

    for (var ci = 0; ci < ColumnIndex; ci++) {
        var th = $(tr).find("th:eq(" + ci + ")");
        if (th.css("display") != "none")
            actualIndex = actualIndex + parseInt($(th).attr("colspan"));
    }

    _floattable_H_actualindex.push({
        "rowindex": RowIndex,
        "cellindex": ColumnIndex,
        "ActualIndex": actualIndex,
        "colspan": parseInt($(currenttd).attr("colspan")),
        "rowspan": parseInt($(currenttd).attr("rowspan"))
    });
    return actualIndex;
}

function InitialData(RowIndex, ColumnIndex) {
    var currenttd = $(tabBody).find("tr:eq(" + RowIndex + ")").find("td:eq(" + ColumnIndex + ")");
    if (!(parseInt($(currenttd).attr("colspan")) > 1 || currenttd.css("display") == "none")) {


        if (RowIndex == 0 && ColumnIndex == 0) {
            _floattable_B_actualindex.push({
                "rowindex": RowIndex,
                "cellindex": ColumnIndex,
                "ActualIndex": 0,
                "innerWidth": $(currenttd).innerWidth(),
                "width": $(currenttd).width(),
                "colspan": parseInt($(currenttd).attr("colspan")),
                "rowspan": parseInt($(currenttd).attr("rowspan"))
            });
        }
        else {


            var latestactualIndex = 0;
            for (var ch = 0; ch < _floattable_B_actualindex.length; ch++) {
                if (_floattable_B_actualindex[ch].rowindex == RowIndex) {
                    latestactualIndex = _floattable_B_actualindex[ch].ActualIndex + _floattable_B_actualindex[ch].colspan;
                }
            }

            var actualIndex = 0;

            for (var ri = 0; ri < _floattable_B_actualindex.length; ri++) {
                if (_floattable_B_actualindex[ri].rowindex < RowIndex && _floattable_B_actualindex[ri].rowspan > 1
                    && ((_floattable_B_actualindex[ri].rowindex + _floattable_B_actualindex[ri].rowspan) > RowIndex)
                    && _floattable_B_actualindex[ri].ActualIndex <= latestactualIndex) {
                    actualIndex = actualIndex + _floattable_B_actualindex[ri].colspan;
                    if (latestactualIndex <= actualIndex) {
                        latestactualIndex = actualIndex;
                    }
                }
            }
            var tr = $(tabBody).find("tr:eq(" + RowIndex + ")");

            for (var ci = 0; ci < ColumnIndex; ci++) {
                var th = $(tr).find("td:eq(" + ci + ")");
                if (th.css("display") != "none")
                    actualIndex = actualIndex + parseInt($(th).attr("colspan"));
            }

            var isnew = true;
            for (var ch = 0; ch < _floattable_B_actualindex.length; ch++) {
                if (_floattable_B_actualindex[ch].ActualIndex == actualIndex) {
                    isnew = false;
                    break;
                }
            }

            if (isnew) {
                _floattable_B_actualindex.push({
                    "rowindex": RowIndex,
                    "cellindex": ColumnIndex,
                    "ActualIndex": actualIndex,
                    "innerWidth": $(currenttd).innerWidth(),
                    "width": $(currenttd).width(),
                    "colspan": parseInt($(currenttd).attr("colspan")),
                    "rowspan": parseInt($(currenttd).attr("rowspan"))
                });
            }
        }
    }
}
