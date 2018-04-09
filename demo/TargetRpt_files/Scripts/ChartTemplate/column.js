

$('#container').highcharts({
    chart: {
        type: 'column'
    },

    colors: ['#4572a7', '#aa4643', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655',
'#FFF263', '#6AF9C4'],


    credits: { enabled: false }
                ,
    title: {
        text: '{#projectName#}2014年{#budgetItemName#} (单位:万元)',
        style: {
            color: '#3E576F',
            fontSize: '16px',
            fontWeight: 'bold'
        }
    },
    subtitle: {
        text: ' '

    },
    xAxis: {
        categories: [
            '1月',
            '2月',
            '3月',
            '4月',
            '5月',
             '6月',
            '7月',
            '8月',
            '9月',
            '10月',
             '11月',
            '12月'
        ]
    },
    yAxis: {
       
        min: 0,
        title: {
            text: ''
        }
    },
    tooltip: {
        headerFormat: '<span style="font-size:16px">{point.key}</span><table>',
        pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
            '<td style="padding:0"><b>{point.y:.1f} 万元</b></td></tr>',
        footerFormat: '</table>',
        shared: true,
        useHTML: true
    },
    plotOptions: {
        column: {
            pointPadding: 0.2,
            borderWidth: 0
        }
    },
    series: '{#series#}'
});