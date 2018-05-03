$(function () {
    new Vue({
        el: "#ReportMonthTemplateContent",
        data: {
            result: {},
            title: "",
            currentState: false,
            totalState: false,
            yearlyState: false,
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
                        "IncludeHaveDetail": false,
                        "IsLatestVersion": true,
                        "DataSource": "Draft",
                        "IsAll": true
                    },
                    success: function (res) {
                        if (res.IsSuccess && res.StatusCode == 200) {
                            self.ShowFilter(res.Data.list[0]);
                            self.title = res.Data.title;
                        } else {
                            utils.alertMessage(res.StatusMessage)
                        }
                    }
                });
            },
            ShowFilter: function (data) {
                var self = this;
                data.ObjValue.forEach(function (one) {
                    if (one.Mark == "CompanyProperty") {
                        one.ObjValue.forEach(function (val) {
                            val.IsCurrentShow = false;
                            val.IsTotalShow = false;
                            val.IsYearShow = false;
                            if (val.ObjValue.length) {
                                val.ObjValue.forEach(function (item) {
                                    item.IsCurrentShow = false;
                                    item.IsTotalShow = false;
                                    item.IsYearShow = false;                                })
                            }
                        })
                    }
                });
                self.result = data;
            },
            ToThousands: function (num) {
                return (parseInt(num) || 0).toString().replace(/(\d)(?=(?:\d{3})+$)/g, '$1,');
            },
        },
        
    })
})