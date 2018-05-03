$(function () {
    new Vue({
        el: "#ReportMonthContent",
        data: {
            systemAndYearList: {},
            result: {
                "Name": "月度经营报告",
                "Mark": "",
                "ObjValue": [{
                    "ID": 1,
                    "MonthlyDetailID": "00000000-0000-0000-0000-000000000000",
                    "TargetID": "999d4230-9280-42f3-925d-e96ec4db19e3",
                    "SystemID": "50643d29-25d0-4c7b-af6e-aac2e82863ee",
                    "FinYear": 2018,
                    "TargetName": "经营收入",
                    "MeasureRate": "18000.0000000",
                    "MeasureRate1": "0.0000000",
                    "NPlanAmmount": "1500.0000000",
                    "NActualAmmount": "2600.0000000",
                    "NDifference": "1100.0000000",
                    "NActualRate": "173%",
                    "NAccumulativePlanAmmount": "4500.0000000",
                    "NAccumulativeActualAmmount": "2600.0000000",
                    "NAccumulativeDifference": "-1900.0000000",
                    "NAccumulativeActualRate": "58%",
                    "NAnnualCompletionRate": "14%",
                    "Counter": 0,
                    "NPlanStr": null,
                    "NActualStr": null,
                    "NAccumulativePlanStr": null,
                    "NAccumulativeActualStr": null,
                    "NDifferStr": null,
                    "NAccDiffereStr": null,
                    "FinMonth": 0,
                    "SystemName": null
                }, {
                    "ID": 2,
                    "MonthlyDetailID": "00000000-0000-0000-0000-000000000000",
                    "TargetID": "0d6abf45-4840-438e-851b-42b2d6efead2",
                    "SystemID": "50643d29-25d0-4c7b-af6e-aac2e82863ee",
                    "FinYear": 2018,
                    "TargetName": "EBITDA",
                    "MeasureRate": "19800.0000000",
                    "MeasureRate1": "0.0000000",
                    "NPlanAmmount": "1650.0000000",
                    "NActualAmmount": "2720.0000000",
                    "NDifference": "1070.0000000",
                    "NActualRate": "165%",
                    "NAccumulativePlanAmmount": "4950.0000000",
                    "NAccumulativeActualAmmount": "2720.0000000",
                    "NAccumulativeDifference": "-2230.0000000",
                    "NAccumulativeActualRate": "55%",
                    "NAnnualCompletionRate": "14%",
                    "Counter": 0,
                    "NPlanStr": null,
                    "NActualStr": null,
                    "NAccumulativePlanStr": null,
                    "NAccumulativeActualStr": null,
                    "NDifferStr": null,
                    "NAccDiffereStr": null,
                    "FinMonth": 0,
                    "SystemName": null
                }, {
                    "ID": 3,
                    "MonthlyDetailID": "00000000-0000-0000-0000-000000000000",
                    "TargetID": "c57e2625-0063-46bf-a9d9-4984416fc2cf",
                    "SystemID": "50643d29-25d0-4c7b-af6e-aac2e82863ee",
                    "FinYear": 2018,
                    "TargetName": "回款额",
                    "MeasureRate": "21600.0000000",
                    "MeasureRate1": "0.0000000",
                    "NPlanAmmount": "1800.0000000",
                    "NActualAmmount": "2560.0000000",
                    "NDifference": "760.0000000",
                    "NActualRate": "142%",
                    "NAccumulativePlanAmmount": "5400.0000000",
                    "NAccumulativeActualAmmount": "2560.0000000",
                    "NAccumulativeDifference": "-2840.0000000",
                    "NAccumulativeActualRate": "47%",
                    "NAnnualCompletionRate": "12%",
                    "Counter": 0,
                    "NPlanStr": null,
                    "NActualStr": null,
                    "NAccumulativePlanStr": null,
                    "NAccumulativeActualStr": null,
                    "NDifferStr": null,
                    "NAccDiffereStr": null,
                    "FinMonth": 0,
                    "SystemName": null
                }, {
                    "ID": 4,
                    "MonthlyDetailID": "00000000-0000-0000-0000-000000000000",
                    "TargetID": "1a58b405-c438-414c-924e-2ee6e9385a24",
                    "SystemID": "50643d29-25d0-4c7b-af6e-aac2e82863ee",
                    "FinYear": 2018,
                    "TargetName": "经营率",
                    "MeasureRate": "23400.0000000",
                    "MeasureRate1": "0.0000000",
                    "NPlanAmmount": "1950.0000000",
                    "NActualAmmount": "3010.0000000",
                    "NDifference": "1060.0000000",
                    "NActualRate": "154%",
                    "NAccumulativePlanAmmount": "5850.0000000",
                    "NAccumulativeActualAmmount": "3010.0000000",
                    "NAccumulativeDifference": "-2840.0000000",
                    "NAccumulativeActualRate": "51%",
                    "NAnnualCompletionRate": "13%",
                    "Counter": 0,
                    "NPlanStr": null,
                    "NActualStr": null,
                    "NAccumulativePlanStr": null,
                    "NAccumulativeActualStr": null,
                    "NDifferStr": null,
                    "NAccDiffereStr": null,
                    "FinMonth": 0,
                    "SystemName": null
                }, {
                    "ID": 5,
                    "MonthlyDetailID": "00000000-0000-0000-0000-000000000000",
                    "TargetID": "acb7a8d6-1d23-4282-8fe8-cfb4cc1d1af6",
                    "SystemID": "50643d29-25d0-4c7b-af6e-aac2e82863ee",
                    "FinYear": 2018,
                    "TargetName": "重点事项达成率",
                    "MeasureRate": "25200.0000000",
                    "MeasureRate1": "0.0000000",
                    "NPlanAmmount": "2100.0000000",
                    "NActualAmmount": "3730.0000000",
                    "NDifference": "1630.0000000",
                    "NActualRate": "178%",
                    "NAccumulativePlanAmmount": "6300.0000000",
                    "NAccumulativeActualAmmount": "3730.0000000",
                    "NAccumulativeDifference": "-2570.0000000",
                    "NAccumulativeActualRate": "59%",
                    "NAnnualCompletionRate": "15%",
                    "Counter": 0,
                    "NPlanStr": null,
                    "NActualStr": null,
                    "NAccumulativePlanStr": null,
                    "NAccumulativeActualStr": null,
                    "NDifferStr": null,
                    "NAccDiffereStr": null,
                    "FinMonth": 0,
                    "SystemName": null
                }],
                "_Company": null,
                "GuoupID": null,
                "Value": null,
                "RowSpanCount": 0,
                "TargetGroupCount": 0,
                "SystemName": null,
                "HtmlTemplate": "",
                "BMonthReportDetail": null,
                "IsBlendTarget": false,
                "Senquence": 0,
                "Level": 0
            },
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
                var url = api_url + 'api/Report/GetSystemAndYearList';
                utils.ajax({
                    type: 'GET',
                    url: url,
                    success: function (res) {
                        if (res.IsSuccess && res.StatusCode == 200) {
                            console.log(res.Data);
                            self.systemAndYearList = res.Data;
                            self.initYM();
                        } else {
                            utils.alertMessage(res.StatusMessage)
                        }
                    }
                });
            },
            ChangeData: function () {
                var self = this;
                var url = api_url + 'api/Report/GetMonthList';
                console.log(self.systemID)
                utils.ajax({
                    type: 'GET',
                    url: url,
                    args: { "SystemID": self.systemID, "Year": Number(self.yearSelect), "Month": Number(self.monthSelect)},
                    success: function (res) {
                        if (res.IsSuccess && res.StatusCode == 200) {
                            console.log(res);
                        } else {
                            utils.alertMessage(res.StatusMessage)
                        }
                    }
                });
            },
            initYM: function () {
                var self = this;
                var date = new Date();
                var year = date.getMonth() + 1;
                self.yearSelect = date.getFullYear();
                self.monthSelect = date.getMonth() + 1.
            },
            toThousands: function (num) {
                return (parseInt(num) || 0).toString().replace(/(\d)(?=(?:\d{3})+$)/g, '$1,');
            }
        }
    })
})