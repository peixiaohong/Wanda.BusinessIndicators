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
            systemID: "",
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
                            utils.alertMessage(res.StatusMessage);
                            self.systemAndYearList = [];
                        }
                    }
                });
            },
            ChangeData: function () {
                var self = this;
                if (!self.systemID || !self.yearSelect || !self.monthSelect || !self.versionSelect) {
                    self.title = "";
                    self.list = {};
                    return false;
                }
                var url = api_url + 'Report/GetMonthList';
                utils.ajax({
                    type: 'GET',
                    url: url,
                    args: {
                        "SystemID": self.systemID,
                        "Year": Number(self.yearSelect),
                        "Month": Number(self.monthSelect),
                        "TargetPlanID": self.versionSelect,
                        "IsLatestVersion": false,
                        "DataSource": "Draft",
                        "IsAll": false,
                    },
                    success: function (res) {
                        if (res.IsSuccess && res.StatusCode == 200) {
                            console.log(JSON.parse(res.Data.list));
                            self.title = res.Data.title;
                            self.list = JSON.parse(res.Data.list);
                        } else {
                            utils.alertMessage(res.StatusMessage);
                            self.title = "";
                            self.list = {};
                        }
                    }
                });
            },
            InitYM: function () {
                var self = this;
                self.yearSelect = self.systemAndYearList.SelectYear;
                self.monthSelect = self.systemAndYearList.SelectMonth;
                if (self.systemAndYearList.System.length > 0)
                    self.systemID = self.systemAndYearList.System[0].ID;
                //var date = new Date();
                //self.yearSelect = date.getFullYear();
                //self.monthSelect = date.getMonth() + 1;
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
                                //console.log(res);
                                self.versions = res.Data;
                                self.versionSelect = self.versions[0].ID;
                            }                           
                            self.ChangeData();
                        } else {
                            utils.alertMessage(res.StatusMessage);
                            self.versions = [];
                            self.versionSelect = "";
                        }
                    }
                });
            },
        }
    })
})