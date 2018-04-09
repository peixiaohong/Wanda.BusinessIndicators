function beforeClick(treeId, treeNode) {
    var zTree = $.fn.zTree.getZTreeObj("treeDemo");
    zTree.checkNode(treeNode, !treeNode.checked, null, true);
    return false;
}

function onCheck(e, treeId, treeNode) {
    var zTree = $.fn.zTree.getZTreeObj("treeDemo"),
    nodes = zTree.getCheckedNodes(true),
    v = "";
    for (var i = 0, l = nodes.length; i < l; i++) {
        v += nodes[i].name + ",";
    }
    if (v.length > 0) v = v.substring(0, v.length - 1);

    var $context = $("#menuContent").data("context");
    if ($context) {
        $context.val(v);
    }
    //var cityObj = $("#citySel");
    //cityObj.attr("value", v);
}

function showMenu(sender) {
    var $sender = $(sender);
    var $senderOffset = $(sender).offset();
    $("#menuContent").data("context", $sender);
    $("#menuContent").css({ left: $senderOffset.left + "px", top: $senderOffset.top + $sender.outerHeight() + "px" }).slideDown("fast");

    $("body").bind("mousedown", onBodyDown);
}
function hideMenu() {
    $("#menuContent").fadeOut("fast");
    $("body").unbind("mousedown", onBodyDown);
}
function onBodyDown(event) {
    // todo
    if (!(event.target.id == "menuBtn" || event.target.id == "citySel" || event.target.id == "menuContent" || $(event.target).parents("#menuContent").length > 0)) {
        hideMenu();
    }
}

/*
   <div id="menuContent" class="menuContent" style="display: none; position: absolute; border: solid 1px #808080; background: #fff;">
        <ul id="treeDemo" class="ztree" style="margin-top: 0; width: 180px; height: 300px;"></ul>
    </div>

     var setting = {
        check: {
            enable: true,
            chkboxType: { "Y": "", "N": "" }
        },
        view: {
            dblClickExpand: false
        },
        data: {
            simpleData: {
                enable: true
            }
        },
        callback: {
            beforeClick: beforeClick,
            onCheck: onCheck
        }
    };
*/