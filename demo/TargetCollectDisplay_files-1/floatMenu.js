var oapx = 0;
var curURL = document.URL;

(function (window) {
    var floatMenu = function (settings) {
        return new floatMenu.fn.init(settings);
    };
    floatMenu.consts = {
        ssoHostPrefix: 'sso',
        ssoHostPrefixDev: 'sso4dev',
        ssoDomainList: [
            '.wanda.cn',
            '.wanda.com.cn',
            '.wandafilm.com',
            '.dagexing.com',
            '.wandaperformance.com',
            '.wandahotels.com',
            '.wanda-te.com',

            '.wanda-dev.cn',
            '.wanda-dev.com.cn',
            '.wandafilm-dev.com',
            '.dagexing-dev.com',
            '.wandaperformance-dev.com',
            '.wandahotels-dev.com',
            '.wanda-te-dev.com'
        ],
        InnerVersion1: 1,
        InnerVersion2: 2
    };
    floatMenu.fn = floatMenu.prototype = {
        copyright: "DALIAN WANDA GROUP CO,LTD",
        settings: {
            "id": "wd-fm",
            "morePanelDisplayTime": 500,
            "maxDisplayInBar": 3,
            "profile": null,
            "email": null,
            "innerVersion": 1
        },
        _settings: {
            "id-fm-menu-item-bar": "id-fm-menu-item-bar", /*整个工具条*/
            "id-fm-menu-item-container": "id-fm-menu-item-container", /*动态菜单项容器*/
            "id-fm-menu-item-more": "id-fm-menu-item-more", /*更多*/
            "id-fm-menu-item-more-panel": "id-fm-menu-item-more-panel", /*更多下拉框*/
            "id-fm-menu-item-right": "id-fm-menu-item-right", /*右侧区域*/
            "id-fm-switch": "id-fm-switch", /*系统导航*/
            "id-fm-menu-item-close": "id-fm-menu-item-close", /*关闭导航*/
            "id-fm-menu-item-logout": "id-fm-menu-item-logout", /*退出*/
            "id-fm-menu-item-change-pwd": "id-fm-menu-item-change-pwd", /*修改密码*/
            "id-fm-switch-profile": "id-fm-switch-profile",
            "id-fm-switch-navigate": "id-fm-switch-navigate",
            "id-fm-menu-notify-tips": "id-fm-menu-notify-tips",
            "id-fm-menu-notify-tips-close": "id-fm-menu-notify-tips-close",
            /*菜单项*/
            "menuItems": [],
            /*以下在InnerVersion2中出现*/
            "id-fm-menu-bar-container": "id-fm-menu-bar-container",/*由页面指定容器的floatMenu外层ID*/
            "id-fm-menu-left-bar": "id-fm-menu-left-bar",
            "id-fm-menu-right-bar": "id-fm-menu-right-bar",
            "id-fm-email": "id-fm-email",
            "id-fm-email-anchor": "id-fm-email-anchor",
            "dropItems": {},
            "dropItemsReverse": {}
        },
        version: "2.0", /*Wanda Style*/
        ssoSiteHttpProtocol: "http:",
        ssoSiteHost: "",
        ssoSiteDomain: "",
        init: function (settings) {
            util.extend(this.settings, this._settings);
            util.extend(this.settings, settings);
        },
        load: function () {
            ssoSiteHost = this.getHost();
            ssoSiteDomain = this.getDomain();
            ssoSiteHttpProtocol = this.getHttpProtocol();

            floatMenu.fn.protectData("id-fm-menu-sso-site-domain", ssoSiteDomain);

            var start = document.cookie.indexOf("displayPwdExpiredTip");
            if (start == -1) {
                document.cookie = "displayPwdExpiredTip=true;path=/;domain=" + ssoSiteHost;
            }
            this.initDisplay();
            this.loadMenu();
            this.render();
            this.bindEvent();
            this.loadStatistics();
        },
        initDisplay: function () {
            var jFm = this.getUIInstance();
        },
        render: function () {
            this.renderProfile();
            // this.renderMenuItem();
        },
        bindEvent: function () {
            var THIS = this;

            util.addEventListener(document.getElementById("wd-fm-id-fm-menu-item-container"), "click", function () {
                if (typeof (wd_sso_sessionidname) != "undefined") {
                    var exdate = new Date();
                    exdate.setDate(exdate.getDate() - 1);
                    document.cookie = wd_sso_sessionidname + "=;expires=" + exdate.toGMTString() + ";";
                }
            });
            util.addEventListener(document.getElementById(this.settings["id-fm-menu-item-more"]), "mouseover", function () {
                THIS.intoMorePanel();
            });
            util.addEventListener(document.getElementById(this.settings["id-fm-menu-item-more"]), "mouseout", function () {
                THIS.goOutMorePanel();
            });
            util.addEventListener(document.getElementById(this.settings["id-fm-menu-item-more-panel"]), "mouseover", function () {
                THIS.intoMorePanel();
            });
            util.addEventListener(document.getElementById(this.settings["id-fm-menu-item-more-panel"]), "mouseout", function () {
                THIS.goOutMorePanel();
            });

            util.addEventListener(document.getElementById(this.settings["id-fm-switch-navigate"]), "click", function () {
                THIS.open();
            });
            util.addEventListener(document.getElementById("id-fm-menu-right-close"), "click", function () {
                THIS.close();
            });

            util.addEventListener(document.getElementById("id-fm-menu-left-close"), "click", function () {
                THIS.close();
            });
            // -----
            util.addEventListener(document.getElementById(this.settings["id-fm-menu-item-logout"]), "click", function () {
                THIS.exit();
            });
            util.addEventListener(document.getElementById(this.settings["id-fm-menu-notify-tips-close"]), "click", function () {
                document.cookie = "displayPwdExpiredTip=false;path=/;domain=" + THIS.retrieveData("id-fm-menu-sso-site-domain");
                var displayPwdTips = document.getElementById("wd-fm-id-fm-menu-notify-tips");
                if (displayPwdTips) {
                    displayPwdTips.style.display = "none";
                }
            });
        },
        bindEventAfterLoad: function () {
            var THIS = this;
            var dropItems = THIS.settings.dropItems;
            var dropItemsReverse = THIS.settings.dropItemsReverse;
            if (dropItems && dropItemsReverse) {
                for (var i in dropItems) {
                    var categoryId = dropItems[i]["categoryId"];
                    var dropItemContainerId = dropItems[i]["dropItemContainerId"];
                    function switchDrop(container, drop) {
                        if (container && drop) {
                            // 如果是关闭的状态
                            if (util.ui.hasClass(drop, "fm-menu-item-drop")) {
                                util.ui.replaceClass(drop, "fm-menu-item-drop", "fm-menu-item-drop-display");
                                if (!util.ui.hasClass(container, "fm-menu-left-bar-category-1level-container-drop")) {
                                    util.ui.addClass(container, "fm-menu-left-bar-category-1level-container-drop");
                                }
                            }
                            else {
                                util.ui.replaceClass(drop, "fm-menu-item-drop-display", "fm-menu-item-drop");
                                if (util.ui.hasClass(container, "fm-menu-left-bar-category-1level-container-drop")) {
                                    util.ui.removeClass(container, "fm-menu-left-bar-category-1level-container-drop");
                                }
                            }
                        }
                    }
                    util.addEventListener(document.getElementById(categoryId), "mouseover", function (e) {
                        var categorySpan = util.event.getTarget(e);
                        if (categorySpan && dropItems[categorySpan.id]) {
                            var dropItemContainerId = dropItems[categorySpan.id]["dropItemContainerId"];
                            var eventDropId = dropItems[categorySpan.id]["dropId"];
                            switchDrop(document.getElementById(dropItemContainerId), document.getElementById(eventDropId));
                        }
                    });
                    util.addEventListener(document.getElementById(dropItemContainerId), "mouseleave", function (e) {
                        var dropItemContainerDiv = util.event.getTarget(e);
                        if (dropItemContainerDiv && dropItemsReverse[dropItemContainerDiv.id]) {
                            var dropItemContainerId = dropItemsReverse[dropItemContainerDiv.id]["dropItemContainerId"];
                            var eventDropId = dropItemsReverse[dropItemContainerDiv.id]["dropId"];
                            if (util.ui.hasClass(dropItemContainerDiv, "fm-menu-left-bar-category-1level-container-drop")) {
                                switchDrop(dropItemContainerDiv, document.getElementById(eventDropId));
                            }
                        }
                    });
                }
            }
        },
        protectData: function (key, value) {
            if (key) {
                var input = document.createElement("input");
                input.type = "hidden";
                input.id = key;
                input.value = value;
                document.body.appendChild(input);
            }
        },
        retrieveData: function (key) {
            var input = document.getElementById(key);
            if (input) {
                return input.value;
            }
        },
        getUIInstance: function () {
            var jFm = document.getElementById(this.settings.id);

            if (util.string.isNullOrEmpty(this.settings.id)) {
                this.settings.id = "wd-fm-" + Math.round(Math.random() * 1000000);
            }
            if (!jFm) {
                this.settings["innerVersion"] = floatMenu.consts.InnerVersion1;
                var id = this.settings.id;
                jFm = document.createElement("div");
                jFm.className = "fm";
                jFm.id = id;
                this._settings["id-fm-menu-item-bar"] = id + "-" + this._settings["id-fm-menu-item-bar"];
                this._settings["id-fm-menu-item-container"] = id + "-" + this._settings["id-fm-menu-item-container"];
                this._settings["id-fm-menu-item-more"] = id + "-" + this._settings["id-fm-menu-item-more"];
                this._settings["id-fm-menu-item-more-panel"] = id + "-" + this._settings["id-fm-menu-item-more-panel"];
                this._settings["id-fm-menu-item-right"] = id + "-" + this._settings["id-fm-menu-item-right"];
                this._settings["id-fm-switch"] = id + "-" + this._settings["id-fm-switch"];
                this._settings["id-fm-menu-item-close"] = id + "-" + this._settings["id-fm-menu-item-close"];
                this._settings["id-fm-menu-item-logout"] = id + "-" + this._settings["id-fm-menu-item-logout"];
                this._settings["id-fm-menu-item-change-pwd"] = id + "-" + this._settings["id-fm-menu-item-change-pwd"];
                this._settings["id-fm-switch-profile"] = id + "-" + this._settings["id-fm-switch-profile"];
                this._settings["id-fm-switch-navigate"] = id + "-" + this._settings["id-fm-switch-navigate"];
                this._settings["id-fm-menu-notify-tips"] = id + "-" + this._settings["id-fm-menu-notify-tips"];
                this._settings["id-fm-menu-notify-tips-close"] = id + "-" + this._settings["id-fm-menu-notify-tips-close"];

                var innerHTML = "<div id=\"" + this._settings["id-fm-menu-item-bar"] + "\" class=\"fm-bar fm-bg\"  style=\"\">                       "
                //                           + "      <div class=\"fm-shadow-top\"></div>                                                                                           "
                //                           + "      <div class=\"fm-shadow-left\"></div>  
                               + "      <div id=\"id-fm-menu-left-close\" class=\"fm-logo-left\"></div>                                                                                        "
                               + "      <div class=\"fm-logo\" ></div>                                                                                                 "
                               + "      <div id=\"" + this._settings["id-fm-menu-item-container"] + "\" class=\"fm-menu-item\">                                   "
                //                + "       <div class=\"fm-menu-item-3\"><a href=\"#\">模块化</a></div>                                                          "
                //                + "       <div class=\"fm-menu-item-4\"><a href=\"#\">资金计划</a></div>                                                        "
                //                + "       <div class=\"fm-menu-item-3\"><a href=\"#\">大歌星</a></div>                                                          "
                               + "      </div>"
                               + " <div id=\"toptext\" style=\"display:inline; float:left; text-align:center;width:60px;line-height: 27px;margin-top:0px;\"><div id=\"" + this._settings["id-fm-menu-item-more"] + "\"  style=\"visibility:hidden;cursor:pointer;line-height: 27px;font-size:12px; vertical-align:middle;\"><span>更多▼</span></div></div>"
                               + "      <div id=\"" + this._settings["id-fm-menu-item-more-panel"] + "\" class=\"fm-menu-item-more-panel\" style=\"display:none;\"></div>               "
                               + " <div id=\"" + this._settings["id-fm-menu-item-right"] + "\" class=\"fm-menu-item-right\"> "
                                + " <span id=\"id-fm-menu-right-close\" class=\"fm-menu-right-close\">关闭导航</span>"
                                  + "<span id=\"" + this._settings["id-fm-menu-item-change-pwd"] + "\" class=\"fm-menu-item-4 spanright\"><a href=\"http://" + ssoSiteHost + "/passwordupdate.aspx\" target=\"_blank\" >修改密码</a></span>"

                               + "<span id=\"" + this._settings["id-fm-menu-item-logout"] + "\" class=\"fm-menu-item-2 spanright\"><a target=\"_parent\" href=\"wd_sso_logout?action=exit&url=" + curURL + "\">退出</a></span> "

                               + '<div style="clear:both; height:0; font-size:0; line-height:0;"></div>'
                               + '<div id="wd-fm-id-fm-menu-notify-tips" class="fm-menu-notify-tips-1" style="display:none;"><div style=\"margin-left: 140px;margin-top: -10px;\"><img id="' + this._settings["id-fm-menu-notify-tips-close"] + '" src=\"' + ssoSiteHttpProtocol + '//' + ssoSiteHost + '/Images/pwdclose.png\" ></img></div><div id=\"fm-menu-tip-text\" style=\"margin-top: -5px;text-align: center;\" ></div></div>'
                               + "      </div>                                                                                                                       "
                               + "   </div>                                                                                                                          "
                               + "   <div id=\"" + this._settings["id-fm-switch"] + "\" class=\"fm-switch fm-bg1\" style=\"display:none;\"><div id=\"" + this._settings["id-fm-switch-navigate"] + "\" class=\"fm-switch-navigate\"><span>系统导航</span></div><div id=\"" + this._settings["id-fm-switch-profile"] + "\" class=\"fm-switch-profile\" style=\"display:none;\"></div></div>";
                document.body.appendChild(jFm);
            }
            else {
                this.settings["innerVersion"] = floatMenu.consts.InnerVersion2;
                var id = this.settings.id;
                this._settings["id-fm-menu-left-bar"] = id + "-" + this._settings["id-fm-menu-left-bar"];
                this._settings["id-fm-switch-profile"] = id + "-" + this._settings["id-fm-switch-profile"];
                this._settings["id-fm-menu-notify-tips"] = id + "-" + this._settings["id-fm-menu-notify-tips"];
                this._settings["id-fm-menu-notify-tips-close"] = id + "-" + this._settings["id-fm-menu-notify-tips-close"];

                floatMenu.fn.protectData("id-fm-menu-notify-tips-close", this._settings["id-fm-menu-notify-tips-close"]);

                var innerHTML = '            <div id=\"' + this._settings["id-fm-menu-left-bar"] + '\" class="fm-menu-left-bar"></div>'
+ '<div id=\"' + this._settings["id-fm-menu-right-bar"] + '\" class="fm-menu-right-bar"><span id=\"' + this._settings["id-fm-switch-profile"] + '\" style=\"display:none;\"></span><span class="fm-menu-right-bar-pwdupdate"><a target=\"_blank\" href="http://' + ssoSiteHost + '/passwordupdate.aspx">修改密码</a></span>'
+ '<span class="fm-menu-right-bar-exit"><a target=\"_parent\" href="wd_sso_logout?action=exit&url=' + curURL + '">退出</a></span></div>';
                 
                innerHTML += '<div id="wd-fm-id-fm-menu-notify-tips" class="fm-menu-notify-tips" style="display:none;">'
+'	<div style="margin-left: 140px;margin-top: -10px;">                                                                  '    
+ '		<img id="' + this._settings["id-fm-menu-notify-tips-close"] + '" src=\"' + ssoSiteHttpProtocol + '//' + ssoSiteHost + '/Images/pwdclose.png\"></img>'
+'	</div>                                                                                                                 '
+'	<div id="fm-menu-tip-text" style="margin-top: -5px;text-align: center;" ></div>                                             '
+'	</div>'
+ '</div>';

                innerHTML = '<div class="fm-menu-wrapper">' + innerHTML + '</div>';
            }

            jFm.innerHTML = innerHTML;
            var pwdExpire = "-1";
            if (typeof (window["wd_sso_passwordExpireDays"]) != "undefined") {
                pwdExpire = window["wd_sso_passwordExpireDays"];
            }

            var displayPwdExpiredTip = util.cookies.getValueByName("displayPwdExpiredTip");
            if (displayPwdExpiredTip == "true" && pwdExpire != "-1" && pwdExpire != "0") {
                var url = "http://" + ssoSiteHost + "/passwordupdate.aspx";
                var text = "修改密码";
                document.getElementById("fm-menu-tip-text").innerHTML = "您的密码还有" + pwdExpire + "天过期，请" + "<a target=\"_blank\" href=" + url + ">" + text + "</a>";
                var displayPwdTips = document.getElementById("wd-fm-id-fm-menu-notify-tips");
                if (displayPwdTips) {
                    displayPwdTips.style.display = ""
                }
            }
            else if (displayPwdExpiredTip == "true" && pwdExpire == "0") {
                var url = "http://" + ssoSiteHost + "/passwordupdate.aspx";
                var text = "修改密码";
                document.getElementById("fm-menu-tip-text").innerHTML = "您的密码今天过期，请" + "<a target=\"_blank\" href=" + url + ">" + text + "</a>";
                var displayPwdTips = document.getElementById("wd-fm-id-fm-menu-notify-tips");
                if (displayPwdTips) {
                    displayPwdTips.style.display = ""
                }
            }
            else {
                var displayPwdTips = document.getElementById("wd-fm-id-fm-menu-notify-tips");
                if (displayPwdTips) {
                    displayPwdTips.style.display = "none";
                }
            }

            util.extend(this.settings, this._settings);
            return jFm;
        },
        _loadMenuData: function () {
            var timer = null;
            var THIS = this;
            var loadMenuScriptInner = function () {
                var menuUrl = null;
                if (THIS.settings["innerVersion"] == floatMenu.consts.InnerVersion1) {
                    menuUrl = ssoSiteHttpProtocol + "//" + ssoSiteHost + "/GetSSOMenus.ashx";
                }
                else if (THIS.settings["innerVersion"] == floatMenu.consts.InnerVersion2) {
                    menuUrl = ssoSiteHttpProtocol + "//" + ssoSiteHost + "/GetSSOMenus.ashx?Version=2";
                }
                var script = document.createElement("script");
                script.src = menuUrl;
                script.type = "text/javascript";
                script.id = "wd_sso_menu_data_script";
                document.getElementsByTagName("head")[0].appendChild(script);
                if (document.getElementById('wd_sso_menu_data_script')) {
                    clearTimeout(timer);
                }
                else {
                    timer = setTimeout(loadMenuDataInner, 50);
                }
            };
            loadMenuScriptInner();
        },
        loadMenu: function () {
            // convert data to render data.
            if (typeof (wd_sso_menuInfo) != "undefined" && wd_sso_menuInfo != null && wd_sso_menuInfo != "") {
                var THIS = this;
                var sysInfo = "";
                var sysInfoList = [];
                this._loadMenuData();
                var timer = null;
                var loadMenuDataInner = function () {
                    if (window.wd_all_sso_menuInfo) {

                        if (window.wd_sso_menuInfo_server != 'undefined') {
                            wd_sso_menuInfo = window.wd_sso_menuInfo_server;
                        }

                        if (THIS.settings["innerVersion"] == floatMenu.consts.InnerVersion1) {
                            // load Menu
                            var data = window.wd_all_sso_menuInfo;
                            var menuData = {};
                            var menuItems = data.split(";");
                            if (menuItems) {
                                for (var i = 0; i < menuItems.length; ++i) {
                                    var items = menuItems[i].split(",");
                                    if (items.length == 5) {
                                        var systemCode = items[0];
                                        var name = items[1];
                                        var isDisplay = items[2];
                                        var orderIndex = items[3];
                                        var systemUrl = items[4];
                                        menuData[systemCode] = {
                                            "systemCode": systemCode,
                                            "name": name,
                                            "isDisplay": isDisplay,
                                            "orderIndex": orderIndex,
                                            "systemUrl": systemUrl
                                        };
                                    }
                                }
                            }

                            var userMenuList = wd_sso_menuInfo.split(";");
                            var userMenuListFull = [];
                            for (var i = 0 ; i < userMenuList.length; ++i) {
                                var systemInfo = userMenuList[i];
                                var systemInfoArray = systemInfo.split(",");
                                var systemCode = systemInfoArray[0];
                                var isDisplay = systemInfoArray[2];
                                var item = menuData[systemCode];
                                if (item) {
                                    var itemStr = item.systemCode + "," + item.name + "," + isDisplay + "," + item.orderIndex + "," + item.systemUrl;
                                    userMenuListFull[userMenuListFull.length] = itemStr;
                                }
                            }
                            var wd_sso_menuInfo1 = userMenuListFull.join(";");

                            var urlPrefix = "wd_sso_logout?action=goto&url=";
                            if (typeof (wd_sso_menuInfo1) != "undefined" && wd_sso_menuInfo1 != null && wd_sso_menuInfo1.length != 0) {
                                sysInfo = wd_sso_menuInfo1;
                                sysInfoList = sysInfo.split(';');
                                var sysList = [];
                                var item = {};
                                if (sysInfoList.length != 0) {
                                    THIS.clearMenuItem();
                                    for (var i = 0; i < sysInfoList.length; i++) {
                                        sysList = sysInfoList[i].split(',');
                                        if (sysList.length >= 5) {
                                            if (sysList[2].toString() == "1") {
                                                item = { "text": decodeURI(sysList[1].toString()), "url": urlPrefix + sysList[4].toString() };
                                                THIS.appendMenuItem(item);
                                            }
                                        }
                                    };
                                }
                            }
                        }
                        else if (THIS.settings["innerVersion"] == floatMenu.consts.InnerVersion2) {
                            // 1、组织用户自己的数据：
                            /*
                            userMenuData = {
                                "GRP001":{"systemCode":"GRP001", "isDisplay":true},
                                "GRP002":{"systemCode":"GRP002", "isDisplay":true}
                            }
                            */
                            var userMenuData = {};
                            var userMenuList = wd_sso_menuInfo.split(";");
                            for (var i = 0 ; i < userMenuList.length; ++i) {
                                var systemInfo = userMenuList[i];
                                var systemInfoArray = systemInfo.split(",");
                                var systemCode = systemInfoArray[0];
                                var isDisplay = systemInfoArray[2] == 1 ? true : false;
                                var item = userMenuData[systemCode];
                                if (!item) {
                                    userMenuData[systemCode] = {
                                        "systemCode": systemCode,
                                        "isDisplay": isDisplay
                                    };
                                }
                            }

                            var data = window.wd_all_sso_menuInfo;
                            var connectedSystem = data["connectedSystem"];
                            var systemCategoryRel = data["systemCategoryRel"];
                            var systemCategory = data["systemCategory"];
                            if (userMenuData && connectedSystem) {
                                // result
                                var displayMenuItems = {
                                    "systemCategory": {},
                                    "systemCategoryRel": {},
                                    "connectedSystem": {},
                                    "oneLevelSystems": {}
                                };

                                for (var systemCode in connectedSystem) {
                                    var system = connectedSystem[systemCode];
                                    var rel = systemCategoryRel[systemCode];

                                    var exists = userMenuData[systemCode];
                                    if (exists && exists["isDisplay"] && system && system["Display"]) {
                                        displayMenuItems["connectedSystem"][systemCode] = system;
                                        // 没有归属关系的系统属于一级系统
                                        if (rel/*[3，5，6，10]*/) {
                                            displayMenuItems["systemCategoryRel"][systemCode] = rel;
                                            for (var i = 0, length = rel.length; i < length; ++i) {
                                                var categoryId = rel[i];
                                                if (!displayMenuItems["systemCategory"][categoryId]) {
                                                    var category = systemCategory[categoryId];
                                                    // 有可能因为数据错误导致一些系统的分类不存在
                                                    if (category) {
                                                        displayMenuItems["systemCategory"][categoryId] = category;

                                                        // 如果category是二级分类的话，则
                                                        var category1LevelId = category["ParentSystemCategoryId"];
                                                        // 如果是一级分类
                                                        if (category1LevelId == "0") {
                                                            // 直接挂在“其他系统”里面
                                                        }
                                                        else {
                                                            var category1Level = systemCategory[category1LevelId];
                                                            // 是二级分类
                                                            if (category1Level && !displayMenuItems["systemCategory"][category1LevelId]) {
                                                                displayMenuItems["systemCategory"][category1LevelId] = category1Level;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else {
                                            displayMenuItems["oneLevelSystems"][systemCode] = system;
                                        }
                                    }
                                }

                                // 转换出可绘制的数据
                                var systemCategoryArray = util.array.to(displayMenuItems["systemCategory"]);
                                // 一级菜单
                                var systemCategory1Levels = util.array.search(systemCategoryArray, "ParentSystemCategoryId", "0");
                                var sortedSystemCategory1Levels = util.array.quicksort(systemCategory1Levels, "OrderIndex");
                                var innerHTML = "信息系统导航：";
                                var counter = 0;
                                if (sortedSystemCategory1Levels && sortedSystemCategory1Levels.length > 0) {
                                    for (var i = 0, length = sortedSystemCategory1Levels.length; i < length; ++i) {
                                        /*
                                          {
                                              "SystemCategoryId": 1,
                                              "CategoryNameCN": "%e8%81%8c%e8%83%bd%e7%b3%bb%e7%bb%9f",
                                              "CategoryNameEN": "%e8%81%8c%e8%83%bd%e7%b3%bb%e7%bb%9f",
                                              "ParentSystemCategoryId": "0",
                                              "OrderIndex": "1"
                                          }
                                        */
                                        var item = sortedSystemCategory1Levels[i];
                                        var idCategory1LevelContainer = THIS.settings.id + "-id-fm-category-1level-" + item["SystemCategoryId"] + "-c" + counter;
                                        var idCategory1Level = idCategory1LevelContainer + "-link";
                                        var idDrop = idCategory1Level + "-drop";
                                        THIS.settings.dropItems[idCategory1Level] = {
                                            "categoryId": idCategory1Level,
                                            "dropId": idDrop,
                                            "dropItemContainerId": idCategory1LevelContainer,
                                            "systemCategory": item
                                        };
                                        THIS.settings.dropItemsReverse[idCategory1LevelContainer] = {
                                            "categoryId": idCategory1Level,
                                            "dropId": idDrop,
                                            "dropItemContainerId": idCategory1LevelContainer,
                                            "systemCategory": item
                                        };
                                        ++counter;

                                        // 创建子分类
                                        // 父分类等于该分类的子分类
                                        var systemCategory2Levels = util.array.search(systemCategoryArray, "ParentSystemCategoryId", item["SystemCategoryId"]);
                                        var sortedSystemCategory2Levels = util.array.quicksort(systemCategory2Levels, "OrderIndex");

                                        var dropHTML = "";
                                        var m = 0;
                                        for (var j = 0, jLength = sortedSystemCategory2Levels.length; j < jLength; ++j) {
                                            ++m;
                                            var jItem = sortedSystemCategory2Levels[j];
                                            var jCategoryId = jItem["SystemCategoryId"];// 分类编号

                                            var displaySystemCategoryRel = displayMenuItems["systemCategoryRel"];
                                            var displayConnectedSystem = displayMenuItems["connectedSystem"];
                                            var lineItems = [];
                                            for (var jSystemCode in displaySystemCategoryRel) {
                                                var jSystemCodeCategoryRel = displaySystemCategoryRel[jSystemCode];
                                                if (jSystemCodeCategoryRel) {
                                                    if (util.array.exists(jSystemCodeCategoryRel, jCategoryId)) {
                                                        var system = displayConnectedSystem[jSystemCode];
                                                        if (system) {
                                                            lineItems[lineItems.length] = system;
                                                        }
                                                    }
                                                }
                                            }

                                            var lineHTML = "";
                                            var k = 0;
                                            var kCount = lineItems.length;
                                            var sortedOrderLineItems = util.array.quicksort(lineItems, "OrderIndex");
                                            for (var l = 0, lLength = sortedOrderLineItems.length; l < lLength; ++l) {
                                                ++k;
                                                var system = sortedOrderLineItems[l];
                                                var shortNameCN = decodeURIComponent(system["ShortNameCN"]);
                                                var systemNameCN = decodeURIComponent(system["SystemNameCN"]);
                                                var systemUrl = decodeURIComponent(system["SystemUrl"]);
                                                var orderIndex = system["OrderIndex"];
                                                if (k == kCount) {
                                                    lineHTML += '<li class=\"no-bg\" orderIndex=\"' + orderIndex + '\"><a title=\"' + systemNameCN + '\" href="' + systemUrl + '" target=\"_blank\">' + shortNameCN + '</a></li>';
                                                }
                                                else {
                                                    lineHTML += '<li orderIndex=\"' + orderIndex + '\"><a title=\"' + systemNameCN + '\" href="' + systemUrl + '" target=\"_blank\">' + shortNameCN + '</a></li>';
                                                }
                                            }

                                            var jCategoryNameCN = decodeURIComponent(jItem["CategoryNameCN"]);
                                            if (m == jLength) {
                                                dropHTML += '<div class="fm-menu-drop-last-line">                                                                                                     '
        + '                                <span class="fm-menu-item-2level">' + jCategoryNameCN + '：</span>                                                                                       '
        + '                                <div class="fm-menu-item-drop-line-value">                                                                                                              '
        + '                                    <ul class="fm-menu-item-drop-list">                                                                                                         '
        + lineHTML
        + '                                    </ul>                                                                                                                          '
        + '                                </div>                                                                                                                             '
        + '                                <div class="fm-menu-item-clearfloat"></div>                                                                                                     '
        + '                            </div>';
                                            }
                                            else {
                                                dropHTML += '<div class="fm-menu-drop-line">                                                                                                     '
    + '                                <span class="fm-menu-item-2level">' + jCategoryNameCN + '：</span>                                                                                       '
    + '                                <div class="fm-menu-item-drop-line-value">                                                                                                              '
    + '                                    <ul class="fm-menu-item-drop-list">                                                                                                         '
    + lineHTML
    + '                                    </ul>                                                                                                                          '
    + '                                </div>                                                                                                                             '
    + '                                <div class="fm-menu-item-clearfloat"></div>                                                                                                     '
    + '                            </div>';
                                            }
                                        }

                                        var categoryNameCN = decodeURIComponent(item["CategoryNameCN"]);

                                        innerHTML += '<span id=\"' + idCategory1LevelContainer + '\" class="fm-menu-left-bar-category-1level">'
                                            + '<a id=\"' + idCategory1Level + '\" href="javascript:;">' + categoryNameCN + '</a>'
    + '                    <div id=\"' + idDrop + '\" class="fm-menu-item-drop"">'
    + '                        <div class="fm-menu-drop-bg">'
    + dropHTML
    + '                        </div>        '
    + '                        <div class="fm-menu-drop-footer"><img src=\"' + ssoSiteHttpProtocol + '//' + ssoSiteHost + '/images/floatMenu3/dropConFoot.png\" /></div>'
    + '                    </div>        '
    + '              </span>        ';
                                    }
                                }
                                // 一级系统
                                var oneLevelSystems = displayMenuItems["oneLevelSystems"];
                                if (oneLevelSystems) {
                                    var sortedOneLevelSystems = util.array.quicksort(
                                        util.array.to(oneLevelSystems), "OrderIndex");
                                    if (sortedOneLevelSystems) {
                                        var length = sortedOneLevelSystems.length;
                                        // 限制个数（不含电子邮件的个数）
                                        if (length > 8) {
                                            length = 8;
                                        }
                                        for (var i = 0; i < length; ++i) {
                                            var system = sortedOneLevelSystems[i];
                                            if (system) {
                                                var shortNameCN = decodeURIComponent(system["ShortNameCN"]);
                                                var systemUrl = decodeURIComponent(system["SystemUrl"]);
                                                innerHTML += '<span class=\"fm-menu-left-bar-system-1level\">'
                                                    + '<a href=\"' + systemUrl + '\" target=\"_blank\">' + shortNameCN + '<\/a><\/span>';
                                            }
                                        }

                                    }
                                }

                                // Email
                                if (window["wd_sso_profile"]) {
                                    THIS.settings["email"] = window["wd_sso_profile"]["email"];
                                    if (THIS.settings["email"]) {
                                        innerHTML += '<span id=\"id-fm-email\" class=\"fm-menu-left-bar-system-1level\" style=\"display:none;\">'
    + '<a id=\"id-fm-email-anchor\" href=\"#\" target=\"_blank\"><\/a><\/span>';
                                    }
                                }

                                var leftBar = document.getElementById(THIS.settings["id-fm-menu-left-bar"]);
                                if (leftBar) {
                                    leftBar.innerHTML = innerHTML;
                                }
                            }
                        }

                        THIS.renderMenuItem();
                        THIS.renderEmail();
                        THIS.bindEventAfterLoad();
                        if (!timer) {
                            clearTimeout(timer);
                        }
                    }
                    else {
                        timer = setTimeout(loadMenuDataInner, 50);
                    }
                };
                loadMenuDataInner();
            }
        },
        _loadBaiduStatistics: function () {
            var timer = null;
            var THIS = this;
            var loadBaiduStatistics = function () {
                // 如果已经在该页面加载过，则不再加载了
                if (document.getElementById("wd_sso_baidu_statistics_script")) {
                    return;
                }
                var statisticUrl = null;
                if (THIS.isDevEnvironment()) {
                    statisticUrl = "//hm.baidu.com/hm.js?9fa07e36c6ef2172597c28e6a821dba2";
                }
                else {
                    statisticUrl = "//hm.baidu.com/hm.js?085fc38faf3a4fade22b53ddde0f1e04";
                }
                var script = document.createElement("script");
                script.src = statisticUrl;
                script.type = "text/javascript";
                script.id = "wd_sso_baidu_statistics_script";
                document.getElementsByTagName("head")[0].appendChild(script);
                if (document.getElementById('wd_sso_baidu_statistics_script') != 'undefined') {
                    clearTimeout(timer);
                }
                else {
                    timer = setTimeout(loadBaiduStatistics, 50);
                }
            };
            loadBaiduStatistics();
        },
        loadStatistics: function () {
            try {
                this._loadBaiduStatistics();
            }
            catch (e) {
                console.info("执行统计的过程中出现异常");
            }
        },
        appendMenuItem: function (item) {
            if (!this.settings.menuItems) this.settings.menuItems = [];
            this.settings.menuItems[this.settings.menuItems.length] = item;
        },
        renderMenuItem: function () {
            if (this.settings["innerVersion"] == floatMenu.consts.InnerVersion1) {
                var menuItemContainer = document.getElementById(this.settings["id-fm-menu-item-container"]);
                var morePanel = document.getElementById(this.settings["id-fm-menu-item-more-panel"]);
                var menuItems = this.settings.menuItems;
                var innerHTML = "";
                var morePanelInnerHTML = "";
                if (menuItemContainer && menuItems && menuItems.length > 0) {
                    var maxDisplayInBar = parseInt(this.settings.maxDisplayInBar, 10);
                    var barMenuItemCount = Math.min(menuItems.length, maxDisplayInBar);
                    var panelMenuItemCount = menuItems.length - barMenuItemCount;
                    var i = 0;
                    var morePanelCount = 0;
                    for (var length = barMenuItemCount; i < length; ++i) {
                        var item = menuItems[i];
                        var text = item["text"];
                        var url = item["url"];
                        var textLength = util.string.getLength(text);
                        var className = "fm-menu-item-" + textLength;
                        innerHTML += "<a target=\"_parent\" href=" + url + ">" + text + "</a>";
                    }
                    for (var length = menuItems.length; i < length; ++i) {
                        var item = menuItems[i];
                        var text = item["text"];
                        var url = item["url"];
                        morePanelInnerHTML += "<a target=\"_parent\" href=" + url + ">" + text + "</a>";
                        ++morePanelCount;
                    }
                }
                if (menuItemContainer)
                    menuItemContainer.innerHTML = innerHTML;
                if (morePanel)
                    morePanel.innerHTML = morePanelInnerHTML;
                if (menuItems.length > 3) {
                    var morediv = document.getElementById("wd-fm-id-fm-menu-item-more");
                    if (morediv) {
                        morediv.style.visibility = "visible";
                    }
                }

                if (morePanelCount > 15 && morePanelCount <= 30) {
                    util.ui.removeClass(morePanel, 'fm-menu-item-more-panel-3');
                    util.ui.addClass(morePanel, 'fm-menu-item-more-panel-2');
                }
                else if (morePanelCount > 30) {
                    util.ui.removeClass(morePanel, 'fm-menu-item-more-panel-2');
                    util.ui.addClass(morePanel, 'fm-menu-item-more-panel-3');
                }
            }
            else if (this.settings["innerVersion"] == floatMenu.consts.InnerVersion2) {

            }
        },
        clearMenuItem: function () {
            var menuItemContainer = document.getElementById(this.settings["id-fm-menu-item-container"]);
            if (menuItemContainer) {
                menuItemContainer.innerHTML = "";
            }
        },
        renderProfile: function () {
            if (this.settings["innerVersion"] == floatMenu.consts.InnerVersion2) {
                if (this.settings.profile && this.settings.profile.toString) {
                    var profileElem = document.getElementById(this.settings["id-fm-switch-profile"]);
                    if (profileElem) {
                        var innerHTML = "";
                        var text = this.settings.profile.toString();
                        //                    var textLength = util.string.getLength(text);
                        //                    var className = "fm-menu-item-" + textLength;
                        profileElem.innerHTML = text;
                        //                    util.ui.addClass(profileElem, className);
                        profileElem.style.display = "";
                    }
                }
            }
        },
        renderEmail:function(){
            if (this.settings["innerVersion"] == floatMenu.consts.InnerVersion2) {
                if (this.settings.email) {
                    var emailElem = document.getElementById(this.settings["id-fm-email"]);
                    var emailAnchorElem = document.getElementById(this.settings["id-fm-email-anchor"]);
                    if (emailElem && emailAnchorElem) {
                        var displayNameCN = decodeURIComponent(this.settings["email"]["webLoginDisplayNameCN"]);
                        var displayNameEN = decodeURIComponent(this.settings["email"]["webLoginDisplayNameEN"]);
                        var webLoginUrl = decodeURIComponent(this.settings["email"]["webLoginUrl"]);
                        emailAnchorElem.innerHTML = displayNameCN;
                        emailAnchorElem.setAttribute("href", webLoginUrl);
                        emailAnchorElem.setAttribute("title", displayNameEN);
                        emailAnchorElem.setAttribute("target", "_blank");
                        emailElem.style.display = "";
                    }
                }
            }
        },
        showMore: function () {
            /*实际触发“更多”框的方法*/
            var btnMore = document.getElementById(this.settings["id-fm-menu-item-more"]);
            var morePanel = document.getElementById(this.settings["id-fm-menu-item-more-panel"]);
            var spanpanel = document.getElementById("toptext");
            var spans = util.ui.getPosition(spanpanel);
            if (btnMore) {
                var btnMorePosition = util.ui.getPosition(btnMore);
                morePanel.style.display = "";

                var panelBottom = btnMorePosition.bottom;
                var panelRight = btnMorePosition.right; /*fixbug：修正因border导致2像素的问题。*/
                var paneltop = btnMorePosition.top;
                morePanel.style.top = "27px";
                morePanel.style.right = (spans.right + oapx) + "px";
                //morePanel.style.right ="250px";
                util.ui.addClass(btnMore, "fm-menu-item-more-hover");
            }
        },
        hideMore: function () {
            /*实际隐藏“更多”框的方法*/
            var btnMore = document.getElementById(this.settings["id-fm-menu-item-more"]);
            var btnMorePanel = document.getElementById(this.settings["id-fm-menu-item-more-panel"]);
            btnMorePanel.style.display = "none";
            util.ui.removeClass(btnMore, "fm-menu-item-more-hover");
        },
        /*私有变量，true设置打开框，false设置关闭框，至于何时打开和关闭，则由watchMorePanel来控制*/
        _showMorePanel: false,
        intoMorePanel: function () {
            this._showMorePanel = true;
            this.showMore(); // 立即打开
            this.watchMorePanel();
        },
        goOutMorePanel: function () {
            this._showMorePanel = false;
        },
        watchMorePanel: function () {
            var btnMorePanel = document.getElementById(this.settings["id-fm-menu-item-more-panel"]);
            var morePanelDisplayTime = parseInt(this.settings["morePanelDisplayTime"], 10);
            var THIS = this;
            setTimeout(function () {
                if (THIS._showMorePanel && !util.ui.isDisplay(btnMorePanel)) {
                    THIS.showMore();
                    THIS.watchMorePanel();
                }
                else if (!THIS._showMorePanel) {
                    THIS.hideMore();
                }
            }, morePanelDisplayTime);
        },
        open: function () {
            var fmToolbar = document.getElementById(this.settings["id-fm-menu-item-bar"]);
            var fmSwitch = document.getElementById(this.settings["id-fm-switch"]);
            var fm = document.getElementById("wd-fm");
            fmToolbar.style.display = "";
            fmSwitch.style.display = "none";
        },
        close: function () {
            var fmToolbar = document.getElementById(this.settings["id-fm-menu-item-bar"]);
            var fmSwitch = document.getElementById(this.settings["id-fm-switch"]);
            var fm = document.getElementById("wd-fm");
            fmToolbar.style.display = "none";
            fmSwitch.style.display = "";
        },
        exit: function () {
        },
        getHost: function () {
            // only support 2.0
            var sso2host = document.getElementById('WD-SSO-LOGIN-SITE-HOST');
            if (sso2host) {
                return sso2host.getAttribute('value');
            }
            // fix 1.0
            var domain = this.getDomain();
            if (domain && domain.length > 0) {
                if (domain.indexOf('-dev') == -1) {
                    return floatMenu.consts.ssoHostPrefix + domain;
                }
                else {
                    return floatMenu.consts.ssoHostPrefixDev + domain;
                }
            }
            return '';
        },
        getDomain: function () {
            // only support 2.0
            var sso2domain = document.getElementById('WD-SSO-LOGIN-SITE-DOMAIN');
            if (sso2domain) {
                return sso2domain.getAttribute('value');
            }
            // fix 1.0
            var ssoDomainList = floatMenu.consts.ssoDomainList;
            var hostname = window.location.hostname.toLowerCase();
            for (var i = 0; i < ssoDomainList.length; ++i) {
                var host = ssoDomainList[i].toLowerCase();
                if ((hostname.indexOf(host) != -1)
                    && (hostname.indexOf(host) == (hostname.length - host.length))) {
                    return host;
                }
            }
            return '';
        },
        getHttpProtocol: function () {
            return window.location.protocol;
        },
        isDevEnvironment: function () {
            var hostname = window.location.hostname.toLowerCase();
            if (hostname.indexOf("-dev.") != -1)
                return true;
            else
                return false;
        }
    }
    floatMenu.fn.init.prototype = floatMenu.fn;
    var util = floatMenu.util = {
        extend: function (collection, collectionOverride) {
            if (collectionOverride && collection) {
                for (var key in collectionOverride) {
                    collection[key] = collectionOverride[key];
                }
            }
        },
        addEventListener: function (obj, event, method) {
            if (obj) {
                if (obj.attachEvent) {
                    obj.attachEvent("on" + event, method);
                }
                else {
                    obj.addEventListener(event, method, false);
                }
            }
        },
        removeEventListener: function (obj, event, method) {
            if (obj) {
                if (obj.detachEvent) {
                    obj.detachEvent("on" + event, method);
                }
                else {
                    obj.removeEventListener(event, method, false);
                }
            }
        },
        isFunction: function (fn) {
            return typeof fn === "function";
        },
        ajax: {
            createXHR: function () {
                if (typeof XMLHttpRequest != "undefined") {
                    return new XMLHttpRequest();
                }
                else if (typeof ActiveXObject != "undefined") {
                    if (typeof arguments.callee.activeXString != "string") {
                        var versions = ["MSXML2.XMLHttp.6.0", "MSXML2.XMLHttp.3.0", "MSXML2.XMLHttp"];
                        for (var i = 0, len = versions.length; i < len; ++i) {
                            try {
                                var xhr = new ActiveXObject(versions[i]);
                                arguments.callee.activeXString = versions[i];
                                return xhr;
                            }
                            catch (ex) {
                                // pass
                            }
                        }
                    }
                    return new ActiveXObject(arguments.callee.activeXString);
                }
                else {
                    throw new Error("No XHR object available");
                }
            },
            post: function (url, data, success, error) {
                var xhr = util.ajax.createXHR();
                xhr.onreadystatechange = function (event) {
                    if (xhr.readyState == 4) {
                        if ((xhr.status >= 200 && xhr.status < 300) || xhr.status == 304) {
                            if (util.isFunction(success))
                                success(xhr.responseText);
                        }
                        else {
                            if (util.isFunction(error))
                                error(xhr.status);
                        }
                    }
                };
                xhr.open("post", url, true);
                xhr.send(data);
            },
            get: function (url, success, error) {
                var xhr = util.ajax.createXHR();
                xhr.onreadystatechange = function (event) {
                    if (xhr.readyState == 4) {
                        if ((xhr.status >= 200 && xhr.status < 300) || xhr.status == 304) {
                            if (util.isFunction(success))
                                success(xhr.responseText);
                        }
                        else {
                            if (util.isFunction(error))
                                error(xhr.status);
                        }
                    }
                };
                xhr.open("GET", url, false);
                xhr.send(null);
            }
        },
        string: {
            is: function (str) {
                if (str && typeof str === "string") return true;
                return false;
            },
            isNullOrEmpty: function (str) {
                if (str && typeof str === "string" && str.length > 0) return false;
                return true;
            },
            canParseNaN: function (str) {
                try {
                    if (isNaN(str)) {
                        return true;
                    }
                    else {
                        var n = parseFloat(str);
                        if (isNaN(n)) {
                            return true;
                        }
                        else {
                            n = parseInt(str, 10);
                            if(isNaN(n)){
                                return true;
                            }
                        }
                        return false;
                    }
                }
                catch (e) {
                    return false;
                }
            },
            getLength: function (str) {
                if (this.isNullOrEmpty(str)) { return 0; }
                else {
                    if (this.is(str)) {
                        return str.length;
                    }
                    else {
                        return 0;
                    }
                }
            }
        },
        cookies: {
            getValueByName: function (c_name) {
                if (document.cookie.length > 0) {
                    c_start = document.cookie.indexOf(c_name + "=")
                    if (c_start != -1) {
                        c_start = c_start + c_name.length + 1
                        c_end = document.cookie.indexOf(";", c_start)
                        if (c_end == -1) c_end = document.cookie.length
                        return unescape(document.cookie.substring(c_start, c_end))
                    }
                }
                return ""
            }
        },
        ui: {
            getComputedStyle: function (elem) {
                if (elem) {
                    var defaultView = elem.ownerDocument.defaultView,
                        computedStyle = defaultView ? defaultView.getComputedStyle(elem, null) : elem.currentStyle;
                    return computedStyle;
                }
                return null;
            },
            getPosition: function (elem) {
                /*CAUTION!!! It's necessary to consider the border top/left width and css position mode.
                This method only deal with the top:0, left:0 mode. */
                var result = {
                    "top": "",
                    "left": "",
                    "width": "",
                    "height": "",
                    "right": "",
                    "bottom": ""
                };
                if (elem) {
                    var offset = this.getParentPosition(elem);
                    offset.offsetTop += elem.offsetTop
                    offset.offsetLeft += elem.offsetLeft;

                    result.left = offset.offsetLeft ? offset.offsetLeft : 0;
                    result.top = offset.offsetTop ? offset.offsetTop : 0;
                    result.width = elem.clientWidth ? elem.clientWidth : 0;
                    result.height = elem.clientHeight ? elem.clientHeight : 0;
                    var windowSize = this.getWindowSize();
                    result.right = windowSize.width - result.left - result.width;
                    result.bottom = windowSize.height - result.top;
                }
                return result;
            },
            getParentPosition: function (elem, result) {
                function createResult() {
                    return {
                        "offsetTop": 0,
                        "offsetLeft": 0
                    };
                }
                result = result || createResult();
                var offsetParent = elem["offsetParent"];
                if (offsetParent == null || offsetParent == 'undefined' || offsetParent == document.body || offsetParent == document.documentElement) {
                    return createResult();
                }
                else {
                    var parentOffset = this.getParentPosition(offsetParent, result);
                    result.offsetTop = offsetParent.offsetTop + parentOffset.offsetTop;
                    result.offsetLeft = offsetParent.offsetLeft + parentOffset.offsetLeft;

                    return result;
                }
            },
            getWindowSize: function () {
                function createResult() {
                    return { "width": 0, "height": 0 };
                }
                var elem = window;
                var body = null;
                var result = createResult();
                if (elem.document.compatMode === "CSS1Compat") {
                    body = elem.document.documentElement;
                }
                else {
                    body = elem.document.body;
                }
                result.width = body["clientWidth"];
                result.height = body["clientHeight"];
                return result;
            },
            isDisplay: function (elem) {
                if (elem) {
                    if (elem.style.display == "none") return false;
                    return true;
                }
                return false;
            },
            hasClass: function (elem, className) {
                if (elem) {
                    var cssName = elem.className;
                    var classes = cssName.split(" ");
                    var found = false;
                    if (classes && classes instanceof Array) {
                        for (var i = 0, length = classes.length; i < length; ++i) {
                            if (classes[i] == className) {
                                return true;
                            }
                        }
                    }
                }
            },
            addClass: function (elem, className) {
                if (elem) {
                    var cssName = elem.className;
                    var classes = cssName.split(" ");
                    var found = false;
                    if (classes && classes instanceof Array) {
                        for (var i = 0, length = classes.length; i < length; ++i) {
                            if (classes[i] == className) {
                                found = true;
                            }
                        }
                        if (!found) {
                            classes[classes.length] = className;
                        }
                        elem.className = classes.join(" ");
                    }
                }
            },
            removeClass: function (elem, className) {
                if (elem) {
                    var cssName = elem.className;
                    var classes = cssName.split(" ");
                    var found = false;
                    if (classes && classes instanceof Array) {
                        for (var i = 0, length = classes.length; i < length; ++i) {
                            if (classes[i] == className) {
                                found = true;
                                classes[i] = "";
                            }
                        }
                        if (found) {
                            elem.className = classes.join(" ");
                        }
                    }
                }
            },
            replaceClass: function (elem, className1, className2) {
                if (elem) {
                    var cssName = elem.className;
                    var classes = cssName.split(" ");
                    var found = false;
                    if (classes && typeof classes == 'Array') {
                        for (var i = 0, length = classes.length; i < length; ++i) {
                            if (classes[i] == className1) {
                                found = true;
                                classes[i] = className2;
                            }
                        }
                    }
                    else {
                        elem.className = className2;
                    }
                }
            }
        },
        event: {
            getTarget: function (e) {
                e = e ? e : window.event
                return e.srcElement ? e.srcElement : e.target;
            }
        },
        array: {
            /*将某个对象值转换为数组
                如：
                {
                    "aaa": {value:1},
                    "bbb": {value:2} 
                }
                将转换为：
                [{value:1}, {value:2}]
            */
            to: function (obj) {
                var result = [];
                for (var i in obj) {
                    result[result.length] = obj[i];
                }
                return result;
            },
            /*
                单值查询
            */
            exists: function (arr, value) {
                if (arr) {
                    for (var i = 0, length = arr.length; i < length; ++i) {
                        if (arr[i] == value) {
                            return true;
                        }
                    }
                }
                return false;
            },
            /*根据对象的某个属性排序*/
            quicksort: function (arr, sortAttribute) {
                if (arr.length <= 1) { return arr; }
                var pivotIndex = Math.floor(arr.length / 2);
                var pivot = arr.splice(pivotIndex, 1)[0];
                var left = [];
                var right = [];

                for (var i = 0, length = arr.length; i < length; i++) {
                    var lN = arr[i][sortAttribute];
                    if (util.string.canParseNaN(lN)) {
                        lN = parseFloat(lN);
                    }
                    var rN = pivot[sortAttribute];
                    if (isNaN(rN)) {
                        rN = parseFloat(rN);
                    }
                    if (lN < rN) {
                        left.push(arr[i]);
                    } else {
                        right.push(arr[i]);
                    }
                }
                return util.array.quicksort(left, sortAttribute).concat([pivot], util.array.quicksort(right, sortAttribute));
            },
            /*
            查询queryAttribute属性等于查询结果的数组子集。
            例如：1、直接用值表示
                 search(
                    [{"key1":"value1-x", "key2":"value2-x"}
                    ,{"key1":"value1-y", "key2":"value2-y"}
                    ,{"key1":"value1-z", "key2":"value2-y"}], "key2", "value2-y")
                 返回值：[{"key1":"value1-y", "key2":"value2-y"}
                        ,{"key1":"value1-z", "key2":"value2-y"}]
                 2、用查询函数查询
                 search(
                    [{"key1":[0,1,2], "key2":[0,1,2]}
                    ,{"key1":[0,1,2], "key2":[1,2,3]}
                    ,{"key1":"value1-z", "key2":[3,4,5]}], "key2", function(o){
                        var 
                    })
                 返回值：[{"key1":"value1-y", "key2":"value2-y"}
                        ,{"key1":"value1-z", "key2":"value2-y"}]
            */
            search: function (arr, queryAttribute, queryExpression) {
                var result = [];
                if (util.isFunction(queryExpression)) {
                    for (var i = 0, length = arr.length; i < length; i++) {
                        if (queryExpression(arr[i][queryAttribute])) {
                            result[result.length] = arr[i];
                        }
                    }
                }
                else {
                    for (var i = 0, length = arr.length; i < length; i++) {
                        if (arr[i][queryAttribute] == queryExpression) {
                            result[result.length] = arr[i];
                        }
                    }
                }
                return result;
            }
        }
    };
    var floatMenuTools = {
        consts: {
            floatMenuId: 'wd-fm'
        },
        injectFlag: null,
        load: function () {
            floatMenuTools.injectFloatMenu();
        },
        injectFloatMenu: function () {
            var fm = document.getElementById(floatMenuTools.consts.floatMenuId);
            if (fm) {
                if (floatMenuTools.plugins) {
                    for (var i = 0, length = floatMenuTools.plugins.length; i < length; ++i) {
                        var plugin = floatMenuTools.plugins[i];
                        if (plugin && typeof plugin === 'function') {
                            plugin();
                        }
                    }
                }
            }
            else {
                setTimeout(floatMenuTools.injectFloatMenu, 100);
            }
        },
        plugins: []
    };
    if (window) {
        window.$fm = window.floatMenu = floatMenu;
        window.$fmTools = window.floatMenuTools = floatMenuTools;
    }
})(window);

floatMenu.util.addEventListener(window, "load", function () {
    var body = document.body || document.documentElement;
    if (body) {
        var paras = null;
        /*
        var wd_fm_info = {
            profile:{
                userid:"zhangsan1",
                username:"%18%23823892" // encodeURIComponent("张三")
                ……
            }
        }
        */
        var wd_fm_info = window.wd_fm_info;
        if (wd_fm_info) {
            paras = {};
            if (wd_fm_info["profile"]) {
                paras["profile"] = {
                    "userid": null,
                    "username": null,
                    toString: function () {
                        var result = "您好，";
                        if (this.username) {
                            result += decodeURIComponent(this.username);
                        }
                        if (this.userid) {
                            result += "（" + this.userid + "）";
                        }

                        return result;
                    }
                };
                if (wd_fm_info["profile"]["userid"]) {
                    paras["profile"]["userid"] = wd_fm_info["profile"]["userid"];
                }
                if (wd_fm_info["profile"]["username"]) {
                    paras["profile"]["username"] = wd_fm_info["profile"]["username"];
                }
            };
        }

        var fm1 = paras ? $fm(paras) : $fm();

        //add new logic to prepare data.
        if (typeof (wd_sso_menuInfo) != "undefined" && wd_sso_menuInfo != null && wd_sso_menuInfo != "") {
            fm1.load();
        }
    }
});