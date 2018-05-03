$(function () {
    new Vue({
        el: "#ReportMonthContent",
        data: {
            systemAndYearList: {},
            list: {},
            title: {},
            reportState: false,
            currentState: false,
            totalState: false,
            yearlyState: false,
            systemID: "a00ad17d-57da-4f8b-9c60-807a5e83d7a7",
            yearSelect: "",
            monthSelect: ""
        },
        mounted: function () {
            var self = this;
            self.InitData();
        },
        methods: {
            InitData: function () {
                var self = this;
                var url = api_url + 'Report/GetSystemAndYearList';
                utils.ajax({
                    type: 'GET',
                    url: url,
                    success: function (res) {
                        if (res.IsSuccess && res.StatusCode == 200) {
                            self.systemAndYearList = res.Data;
                            self.InitYM();
                            self.ChangeData();
                        } else {
                            utils.alertMessage(res.StatusMessage)
                        }
                    }
                });
            },
            ChangeData: function () {
                var self = this;
                var url = api_url + 'Report/GetMonthList';
                utils.ajax({
                    type: 'GET',
                    url: url,
                    args: {
                        "SystemID": self.systemID,
                        "Year": Number(self.yearSelect),
                        "Month": Number(self.monthSelect),
                        "IsLatestVersion": true,
                        "DataSource": "Draft",
                        "IsAll": true,
                    },
                    success: function (res) {
                        if (res.IsSuccess && res.StatusCode == 200) {
                            self.title = res.Data.title;
                            self.list = JSON.parse(res.Data.list);
                        } else {
                            utils.alertMessage(res.StatusMessage)
                        }
                    }
                });
            },
            InitYM: function () {
                var self = this;
                var date = new Date();
                self.yearSelect = date.getFullYear();
                self.monthSelect = date.getMonth() + 1.
            },
            ToThousands: function (num) {
                return (parseInt(num) || 0).toString().replace(/(\d)(?=(?:\d{3})+$)/g, '$1,');
            },
            Trim: function (str) {
                if (str.length) {
                    return str.replace(/(?!(^\n|\n{2}))\n/g, "<br/>"); 
                } 
            }  
        }
    })
})