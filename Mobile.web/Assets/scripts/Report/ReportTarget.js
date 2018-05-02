$(function () {
    var vm = new Vue({
        el: "#ReportTargetContent",
        data: {
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
        },
        mounted: function () {
            var self = this;
            self.$nextTick(function () {
                var length = self.result[0].TargetDetailList.length + 1;
                utils.initTarget(".target-main", ".target-content", ".target-name", ".target-allow", length)
            })
        },
        methods: {

        },
    })
})