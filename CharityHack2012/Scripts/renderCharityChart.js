(function($) {
    $(document).ready(function () {
        debugger;
        var investmentGains = $("#income").find("[data-investment-gains]").data('investment-gains');
        var investments = $("#income").find("[data-investments]").data('investments');
        var tradingFunds = $("#income").find("[data-trading-funds]").data('trading-funds');
        var voluantaryIncome = $("#income").find("[data-voluantary-income]").data('trading-funds');
        var incomeOther = $("#income").find("[data-income-other]").data('income-other');

        var data = [
          ['Investments', investments], ['Investment Gains', investmentGains], ['Trading Funds', tradingFunds],
          ['Voluantary Income', voluantaryIncome], ['Other Income', incomeOther]
        ];
        jQuery.jqplot('chartdiv', [data],
          {
              seriesDefaults: {
                  // Make this a pie chart.
                  renderer: jQuery.jqplot.PieRenderer,
                  rendererOptions: {
                      // Put data labels on the pie slices.
                      // By default, labels show the percentage of the slice.
                      showDataLabels: true
                  },
              },
              legend: { show: true, location: 'e' }
          }
        );

        $('#income-chart .modal-body').append($('#chartdiv'));
    });
}(jQuery));