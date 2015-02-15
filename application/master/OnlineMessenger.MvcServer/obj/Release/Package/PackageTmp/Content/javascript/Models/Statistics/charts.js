
$(function () {
    postJSONToController("/UsersPerDay", null, function (serverData) {
        var fomatted = [];
        for (var i in serverData) {
            var item = serverData[i];
            fomatted.push([moment.utc(i).valueOf(), item]);
        }

        $('#users-per-day-container').highcharts({
            chart: {
                type: 'spline',
                backgroundColor: null
            },
            title: {
                text: 'Users per day'
            },
            subtitle: {
                text: 'Quantity of users applied to operators per day'
            },
            xAxis: {
                type: 'datetime',
                dateTimeLabelFormats: {
                    month: '%e. %b',
                    year: '%b'
                }
            },
            yAxis: {
                title: {
                    text: 'Amount'
                },
                min: 0
            },
            tooltip: {
                formatter: function () {
                    return '<b>' + this.series.name + '</b><br/>' +
                        Highcharts.dateFormat('%e. %b', this.x) + ': ' + this.y + ' users';
                }
            },

            series: [{
                name: 'Users',
                data: fomatted
            }]
        });


    });
});

$(function () {

    postJSONToController("/MessagesPerDay", null, function (serverData) {
        var fomatted = [];
        for (var i in serverData) {
            var item = serverData[i];
            fomatted.push([moment.utc(i).valueOf(), item]);
        }


        $('#messages-per-day-container').highcharts({
            chart: {
                type: 'area',
                backgroundColor: null
            },
            title: {
                text: 'Users per day'
            },
            subtitle: {
                text: 'Source: <a href="http://thebulletin.metapress.com/content/c4120650912x74k7/fulltext.pdf">' +
                    'thebulletin.metapress.com</a>'
            },
            xAxis: {
                type: 'datetime',
                dateTimeLabelFormats: {
                    month: '%e. %b',
                    year: '%b'
                }
            },
            yAxis: {
                title: {
                    text: 'Amount'
                },
                min: 0
            },
            tooltip: {
                formatter: function () {
                    return '<b>' + this.series.name + '</b><br/>' +
                        Highcharts.dateFormat('%e. %b', this.x) + ': ' + this.y + ' users';
                }
            },
            plotOptions: {
                area: {
                    pointStart: 1940,
                    marker: {
                        enabled: false,
                        symbol: 'circle',
                        radius: 2,
                        states: {
                            hover: {
                                enabled: true
                            }
                        }
                    }
                }
            },
            series: [{
                name: 'Messages count per day',
                data: fomatted
            }]
        });


    });


});


$(function () {

    postJSONToController("/UsersPerOperator", null, function (serverData) {

        var fomatted = [];
        for (var i in serverData) {
            var item = serverData[i];
            fomatted.push([i, item]);
        }


        $('#users-per-operator-container').highcharts({
            chart: {
                plotBackgroundColor: null,
                backgroundColor: null,
                plotBorderWidth: null,
                plotShadow: false
            },
            title: {
                text: 'Users per operator'
            },
            tooltip: {
                pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: true,
                        color: '#000000',
                        connectorColor: '#000000',
                        format: '<b>{point.name}</b>: {point.percentage:.1f} %'
                    }
                }
            },
            series: [{
                type: 'pie',
                name: 'Users per operator',
                data: fomatted
            }]
        });


    });


});
