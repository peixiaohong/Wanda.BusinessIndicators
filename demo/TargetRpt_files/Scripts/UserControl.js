/*  
*@Description: 右键弹出菜单插件  
*@Param obj:   触发菜单区域  
*@Param menu:  菜单对象  
*@Param offX:  偏移x值  
*@Param offX:  偏移y值  
*@Author:      sev7n(2010-11-03)  
*@Update:  
*@resourse:  http://sev7n.net/index.php/572.html
*/

var rightMenu = function (options) {

    var defaults = {
        obj: document, menu: "#context-menu", offX: 5, offY: 5
    },
        options = $.extend({}, defaults, options);

    this.options = options;
    this.options.obj = $(this.options.obj);
    this.options.menu = $(this.options.menu);

    this.init();
};

// 初始   
rightMenu.prototype.init = function () {
    var that = this,
        $obj = that.options.obj,
        $menu = that.options.menu;

    // 屏蔽右键   
    $obj.bind("contextmenu", function () {
        return false;
    });

    // 右键触发菜单弹出   
    $obj.mousedown(function (evt) {
        evt = evt || window.event;
        if (evt.button === 2) {
            that.mouseCoords(evt);
        } else {
            $menu.hide();
        }
    });
}

// 点击坐标   
rightMenu.prototype.mouseCoords = function (evt) {
    var that = this,
        coord = {};

    if (evt.pageX || evt.pageY) {
        coord = { x: evt.pageX, y: evt.pageY };
    } else {
        coord = { x: evt.clientX, y: evt.clientY + document.body.scrollTop };
    }

    that.rightMenuOut(coord);
}

// 菜单弹出   
rightMenu.prototype.rightMenuOut = function (coord) {
    var that = this,
        opts = that.options,
        $menu = opts.menu,
        $window = $(window),
        _menuW = $menu.outerWidth(),
        _menuH = $menu.outerHeight(),
        _winW = $window.width(),
        _winH = $window.height() + document.body.scrollTop,
        _cX = coord.x,
        _cY = coord.y;

    // 靠近边框判断   
    _cX = (_menuW + _cX + opts.offX) < _winW ? (_cX + opts.offX) : (_cX - _menuW - opts.offX);
    _cY = (_menuH + _cY + opts.offY) < _winH ? (_cY + opts.offY) : (_cY - _menuH - opts.offY);

    $menu.css({ 'left': _cX, 'top': _cY }).fadeIn(200).mousedown(function () {
        // 屏蔽菜单上的点击   
        return false;
    });
}





function MenuBuilder(containerid, menuItemArray) {
    this._containerid = containerid;
    this._menuItemArray = [];
    this._menuItemArray.length
    if (typeof (menuItemArray) != undefined) {
        this._menuItemArray = menuItemArray;
    }
    this._menuItemFormate = "<li><a>$Text</a></li>";
}
MenuBuilder.prototype.AddItem = function (menuItem) {
    this._menuItemArray.push(menuItem);
}
MenuBuilder.prototype.BindItem = function (item) {

    var menuItemStr = this._menuItemFormate.replace("$Text", item.Text);
    var menuItem = $(menuItemStr + "");


    menuItem.appendTo($("#" + this._containerid));
    menuItem.click(
                    function () {
                        //alert(menuItemStr);
                        if (typeof (item.Click) != undefined) {
                            item.Click();
                        }
                    }
                );
}
MenuBuilder.prototype.Render = function () {
    $("#" + this._containerid).html("");

    var ele = this;
    for (var i = 0; i < this._menuItemArray.length; i++) {
        this.BindItem(this._menuItemArray[i]);
    }
}