window.onload = function () {
    var config = {
        id: "tg1",
        width: "800",
        renderTo: "div1",
        headerAlign: "left",
        headerHeight: "30",
        dataAlign: "left",
        indentation: "20",
        folderOpenIcon: "images/folderOpen.png",
        folderCloseIcon: "images/folderClose.png",
        defaultLeafIcon: "images/defaultLeaf.gif",
        hoverRowBackground: "false",
        folderColumnIndex: "1",
        itemClick: "itemClickEvent",
        columns: [{
            headerText: "",
            headerAlign: "center",
            dataAlign: "center",
            width: "20"
        },
        {
            headerText: "名称",
            dataField: "name",
            headerAlign: "center",
            handler: "customOrgName"
        },
        {
            headerText: "拼音码",
            dataField: "code",
            headerAlign: "center",
            dataAlign: "center",
            width: "100",
            hidden: true
        },
        {
            headerText: "负责人",
            dataField: "assignee",
            headerAlign: "center",
            dataAlign: "center",
            width: "100"
        },
        {
            headerText: "查看",
            headerAlign: "center",
            dataAlign: "center",
            width: "50",
            handler: "customLook"
        }],
        data: [
            {
                name: "城区分公司",
                code: "CQ",
                assignee: "",
                children: [{
                    name: "城区卡品分销中心"
                },
                {
                    name: "先锋服务厅",
                    children: [{
                        name: "chlid1"
                    },
                    {
                        name: "chlid2"
                    },
                    {
                        name: "chlid3",
                        children: [{
                            name: "chlid3-1",
                            assignee: "chlid3-1的负责人1"
                        },
                        {
                            name: "chlid3-2",
                            assignee: "chlid3-2的负责人2"
                        },
                        {
                            name: "chlid3-3",
                            assignee: "chlid3-3的负责人3"
                        },
                        {
                            name: "chlid3-4",
                            assignee: "chlid3-4的负责人4"
                        }]
                    }]
                },
                {
                    name: "半环服务厅"
                }]
            },
            {
                name: "清新分公司",
                code: "QX",
                assignee: "新增的负责人1",
                children: []
            },
            {
                name: "英德分公司",
                code: "YD",
                assignee: "新增的负责人2",
                children: []
            },
            {
                name: "佛冈分公司",
                code: "FG",
                assignee: "新增的负责人3",
                children: [{
                    name: "节点1", children: [{
                        name: "chlid3-1",
                        assignee: "chlid3-1的负责人1"
                    }]
                }, { name: "节点2" }, { name: "节点3" }, { name: "节点4" }, { name: "节点5", children: [{name:"新增的节点"}]}]
            }]
    };
    debugger;
    var treeGrid = new TreeGrid(config);
    treeGrid.show()
}