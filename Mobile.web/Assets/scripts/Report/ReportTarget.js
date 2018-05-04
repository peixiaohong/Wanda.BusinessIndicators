$(function () {
    var vm = new Vue({
        el: "#ReportTargetContent",
        data: {
            systemAndYearList: {},
            result: [{
                "FinMonth": "月份",
                "TargetDetailList": [{
                    "TargetID": "558cf58e-66cb-4379-9cff-12051f7436f6",
                    "TargetName": "经营收入",
                    "Target": 1600,
                    "SumTarget": 1600
                }, {
                    "TargetID": "cc4aa1ec-d374-4f15-83c9-8b7e15f3a8cd",
                    "TargetName": "经营利润",
                    "Target": 1760,
                    "SumTarget": 1760
                }, {
                    "TargetID": "4deda86f-40f8-4791-a69e-4d980ec8654c",
                    "TargetName": "统一收银额",
                    "Target": 1920,
                    "SumTarget": 1920
                    }, {
                        "TargetID": "558cf58e-66cb-4379-9cff-12051f7436f6",
                        "TargetName": "经营收入",
                        "Target": 1600,
                        "SumTarget": 1600
                    }]
            },
                {
                "FinMonth": 1,
                "TargetDetailList": [{
                    "TargetID": "558cf58e-66cb-4379-9cff-12051f7436f6",
                    "TargetName": "经营收入",
                    "Target": 1600,
                    "SumTarget": 1600
                }, {
                    "TargetID": "cc4aa1ec-d374-4f15-83c9-8b7e15f3a8cd",
                    "TargetName": "经营利润",
                    "Target": 1760,
                    "SumTarget": 1760
                }, {
                    "TargetID": "4deda86f-40f8-4791-a69e-4d980ec8654c",
                    "TargetName": "统一收银额",
                    "Target": 1920,
                    "SumTarget": 1920
                    }, {
                        "TargetID": "558cf58e-66cb-4379-9cff-12051f7436f6",
                        "TargetName": "经营收入",
                        "Target": 1600,
                        "SumTarget": 1600
                    } ]
            }, {
                "FinMonth": 2,
                "TargetDetailList": [{
                    "TargetID": "558cf58e-66cb-4379-9cff-12051f7436f6",
                    "TargetName": "经营收入",
                    "Target": 1600,
                    "SumTarget": 3200
                }, {
                    "TargetID": "cc4aa1ec-d374-4f15-83c9-8b7e15f3a8cd",
                    "TargetName": "经营利润",
                    "Target": 1760,
                    "SumTarget": 3520
                }, {
                    "TargetID": "4deda86f-40f8-4791-a69e-4d980ec8654c",
                    "TargetName": "统一收银额",
                    "Target": 1920,
                    "SumTarget": 3840
                    }, {
                        "TargetID": "558cf58e-66cb-4379-9cff-12051f7436f6",
                        "TargetName": "经营收入",
                        "Target": 1600,
                        "SumTarget": 1600
                    }]
            }, {
                "FinMonth": 3,
                "TargetDetailList": [{
                    "TargetID": "558cf58e-66cb-4379-9cff-12051f7436f6",
                    "TargetName": "经营收入",
                    "Target": 1600,
                    "SumTarget": 4800
                }, {
                    "TargetID": "cc4aa1ec-d374-4f15-83c9-8b7e15f3a8cd",
                    "TargetName": "经营利润",
                    "Target": 1760,
                    "SumTarget": 5280
                }, {
                    "TargetID": "4deda86f-40f8-4791-a69e-4d980ec8654c",
                    "TargetName": "统一收银额",
                    "Target": 1920,
                    "SumTarget": 5760
                    }, {
                        "TargetID": "558cf58e-66cb-4379-9cff-12051f7436f6",
                        "TargetName": "经营收入",
                        "Target": 1600,
                        "SumTarget": 1600
                    }]
            }, {
                "FinMonth": 4,
                "TargetDetailList": [{
                    "TargetID": "558cf58e-66cb-4379-9cff-12051f7436f6",
                    "TargetName": "经营收入",
                    "Target": 1600,
                    "SumTarget": 6400
                }, {
                    "TargetID": "cc4aa1ec-d374-4f15-83c9-8b7e15f3a8cd",
                    "TargetName": "经营利润",
                    "Target": 1760,
                    "SumTarget": 7040
                }, {
                    "TargetID": "4deda86f-40f8-4791-a69e-4d980ec8654c",
                    "TargetName": "统一收银额",
                    "Target": 1920,
                    "SumTarget": 7680
                    }, {
                        "TargetID": "558cf58e-66cb-4379-9cff-12051f7436f6",
                        "TargetName": "经营收入",
                        "Target": 1600,
                        "SumTarget": 1600
                    } ]
                }],
            systemID: "a00ad17d-57da-4f8b-9c60-807a5e83d7a7",
            yearSelect: "",
            head: [],
            list: [],
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
                            self.LoadData();
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
            LoadData: function () {
                var self = this;
                var url = api_url + 'Report/GetVerTargetList';
                utils.ajax({
                    type: 'GET',
                    url: url,
                    args: {
                        "SysID": self.systemID,
                        "FinYear": self.yearSelect
                    },
                    success: function (res) {
                        if (res.IsSuccess && res.StatusCode == 200) {
                            self.head = res.Data.head;
                            self.list = res.Data.list;
                            self.$nextTick(function () {
                                var length = self.head.length + 1;
                                utils.initTarget(".target-main", ".target-content", ".target-name", ".target-allow", length)
                            })
                        } else {
                            utils.alertMessage(res.StatusMessage)
                        }
                    }
                });
            },
            ToThousands: function (num) {
                return (parseInt(num) || 0).toString().replace(/(\d)(?=(?:\d{3})+$)/g, '$1,');
            },
        },
    })
})