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
            monthSelect: "",
            versions: [],
            versionSelect: "",
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
                        "TargetPlanID": self.versionSelect,
                        "IsLatestVersion": true,
                        "DataSource": "Draft",
                        "IsAll": true,
                    },
                    success: function (res) {
                        if (res.IsSuccess && res.StatusCode == 200) {
                            console.log(res);
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
                self.ChangeVersion();
            },
            ChangeVersion: function () {
                var self = this;
                var url = api_url + 'Report/GetTargetPlanVersionList';
                utils.ajax({
                    type: 'GET',
                    url: url,
                    args: {
                        "SystemID": self.systemID,
                        "Year": Number(self.yearSelect),
                        "Month": Number(self.monthSelect),
                    },
                    success: function (res) {
                        if (res.IsSuccess && res.StatusCode == 200) {
                            if (res.Data.length) {
                                console.log(res);
                                self.versions = res.Data;
                                self.versionSelect = self.versions[0].ID;
                            }                           
                            self.ChangeData();
                        } else {
                            utils.alertMessage(res.StatusMessage)
                        }
                    }
                });
            },
        }
    })
})