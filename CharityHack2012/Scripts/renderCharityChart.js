(function($) {
    $(document).ready(function () {
        var r = Raphael(10, 50, 640, 480);
        r.g.pieChart(320, 240, 100, [55, 20, 13, 32, 5, 1, 2]);
    });
}(jQuery));