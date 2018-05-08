$(function () {
    var vm = new Vue({
        el: "#ReportTargetContent",
        data: {
            systemAndYearList: {},
            systemID: "a00ad17d-57da-4f8b-9c60-807a5e83d7a7",
            yearSelect: "",
            head: [],
            list: [],
            versions: [],
            versionSelect: "",
            reportState: true,
            length:""
        },
        mounted: function () {
            var self = this;
            self.InitYM();
        },
        methods: {
            InitYM: function () {
                var self = this;
                var url = api_url + 'Report/GetTargetCollectInit';
                utils.ajax({
                    type: 'GET',
                    url: url,
                    success: function (res) {
                        if (res.IsSuccess && res.StatusCode == 200) {
                            self.systemAndYearList = res.Data;
                            var date = new Date();
                            self.yearSelect = date.getFullYear().toString();
                            self.InitVersion();
                        } else {
                            utils.alertMessage(res.StatusMessage)
                        }
                    }
                });
            },
            ChangeData: function () {
                var self = this;
                self.LoadData();
            },
            InitVersion: function () {
                var self = this;
                var url = api_url + 'Report/GetTargetPlanVersionList';
                utils.ajax({
                    type: 'GET',
                    url: url,
                    args: {
                        "SystemID": self.systemID,
                        "Year": self.yearSelect,
                    },
                    success: function (res) {
                        if (res.IsSuccess && res.StatusCode == 200) {
                            if (res.Data.length) {
                                self.versions = res.Data;
                                self.versionSelect = self.versions[0].ID;                              
                            }
                            self.LoadData();
                        } else {
                            utils.alertMessage(res.StatusMessage)
                        }
                    }
                });
            },
            LoadData: function () {
                var self = this;
                var url = api_url + 'Report/GetTargetPlanList';
                utils.ajax({
                    type: 'GET',
                    url: url,
                    args: {
                        "SysID": self.systemID,
                        "FinYear": self.yearSelect,
                        "TargetPlanID": self.versionSelect,
                    },
                    success: function (res) {
                        if (res.IsSuccess && res.StatusCode == 200) {
                            if (res.Data.head.length != res.Data.list[0].TargetDetailList.length) {
                                self.head = [];
                                self.list = [];
                                return false;
                            }
                            self.head = res.Data.head;
                            self.list = res.Data.list;
                            self.$nextTick(function () {
                                self.length = self.head.length + 1;
                                utils.initTarget(".target-main", ".target-content", ".target-name", ".target-allow", self.length)
                            })
                        } else {
                            utils.alertMessage(res.StatusMessage)
                        }
                    }
                });
            },
        },
        watch: {
            reportState: function (val) {
                var self = this;
                self.$nextTick(function () {
                    utils.initTarget(".target-main", ".target-content", ".target-name", ".target-allow", self.length)
                })
            }
        }
    })
})