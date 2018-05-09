$(function () {
    new Vue({
        el: "#ReportMonthTemplateContent",
        data: {
            result: {},
            title: "",
            currentState: false,
            totalState: false,
        },   
        mounted: function () {
            var self = this;
            self.InitData();
        },
        methods: {
            InitData: function () {
                var self = this;
                var url = api_url + 'Report/GetMonthDetailList';
                utils.ajax({
                    type: 'GET',
                    url: url,
                    args: {
                        "SystemID": utils.getQueryString("id"),
                        "Year": Number(utils.getQueryString("year")),
                        "Month": Number(utils.getQueryString("month")),
                        "TargetName": decodeURI(utils.getQueryString("name")),
                        "TargetPlanID": utils.getQueryString("versionID"),
                        "IncludeHaveDetail": false,
                        "IsLatestVersion": true,
                        "DataSource": "Draft",
                        "IsAll": true
                    },
                    success: function (res) {
                        if (res.IsSuccess && res.StatusCode == 200) {
                            if (res.Data.list.length) {
                                self.ShowFilter(res.Data.list);
                            }
                            self.title = res.Data.title;
                        } else {
                            utils.alertMessage(res.StatusMessage)
                        }
                    }
                });
            },
            ShowFilter: function (data) {
                var self = this;
                var name = decodeURI(utils.getQueryString("name"));
                var match = {
                    Name: "",
                    ObjValue: {}
                }
                data.forEach(function (one) {
                    if (one.Name != name && one.Name.indexOf(name) != -1) {
                        one.ObjValue.forEach(function (vals) {
                            if (vals.Name == name) {
                                match.Name = vals.Name
                                vals.ObjValue.forEach(function (val) {
                                    if (val.Mark == "CompanyProperty") {
                                        val.ObjValue.forEach(function (items) {
                                            items.IsCurrentShow = false;
                                            items.IsTotalShow = false;
                                            items.IsYearShow = false;
                                            if (items.ObjValue.length) {
                                                items.ObjValue.forEach(function (item) {
                                                    item.IsCurrentShow = false;
                                                    item.IsTotalShow = false;
                                                    item.IsYearShow = false;
                                                })
                                            }
                                        })
                                        match.ObjValue = val.ObjValue
                                    }
                                });
                            }
                        });
                    } else {
                        if (one.Name == name) {
                            match.Name = one.Name
                            one.ObjValue.forEach(function (val) {

                                if (val.Mark == "CompanyProperty") {
                                    val.ObjValue.forEach(function (items) {
                                        items.IsCurrentShow = false;
                                        items.IsTotalShow = false;
                                        items.IsYearShow = false;
                                        if (items.ObjValue.length) {
                                            items.ObjValue.forEach(function (item) {
                                                item.IsCurrentShow = false;
                                                item.IsTotalShow = false;
                                                item.IsYearShow = false;
                                            })
                                        }
                                    });
                                    match.ObjValue = val.ObjValue
                                }
                            });
                        }
                    }
                })
                self.result = match;
            },
        },
        
    })
})