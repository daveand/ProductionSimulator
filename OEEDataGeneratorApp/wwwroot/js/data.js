"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/dataHub").build();

//Disable send button until connection is established
document.getElementById("startSimulation").disabled = true;
document.getElementById("idealRunRate").disabled = true;
document.getElementById("actualRunRateMin").disabled = true;
document.getElementById("actualRunRateMax").disabled = true;

var productionMinuteSpan = document.getElementById("productionMinute");
var timeStampSpan = document.getElementById("timeStamp");
var ackTotalCountSpan = document.getElementById("ackTotalCount");
var ackTotalRejectSpan = document.getElementById("ackTotalReject");

var availabilitySpan = document.getElementById("availability");
var performanceSpan = document.getElementById("performance");
var qualitySpan = document.getElementById("quality");
var oeeSpan = document.getElementById("oee");

//connection.on("ReceiveMessage", function (user, message) {
//    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
//    var encodedMsg = user + " says " + msg;
//    var li = document.createElement("li");
//    li.textContent = encodedMsg;
//    document.getElementById("messagesList").appendChild(li);
//});

var lineChartData;
var ctx = document.getElementById('LineChart').getContext('2d');
var ctxOEE = document.getElementById('OEEChart').getContext('2d');

connection.on("StartSimulation", function (lineChartData) {
    console.log(lineChartData);

    let minute = lineChartData.map(m => m.productionMinute);
    let timeStamp = lineChartData.map(m => m.timeStamp);
    let ackTotalCount = lineChartData.map(m => m.ackTotalCount);
    let ackTotalReject = lineChartData.map(m => m.ackTotalReject);

    productionMinuteSpan.innerHTML = minute[minute.length -1];
    timeStampSpan.innerHTML = timeStamp[timeStamp.length - 1];
    ackTotalCountSpan.innerHTML = parseInt(ackTotalCount[ackTotalCount.length - 1]);
    ackTotalRejectSpan.innerHTML = parseInt(ackTotalReject[ackTotalReject.length - 1]);

    let labels = lineChartData.map(l => l.timeStamp);
    let totalCount = lineChartData.map(d => d.totalCount);
    let rejectCount = lineChartData.map(d => d.rejectCount);

    var LineChart = new Chart(ctx, {
        // The type of chart we want to create
        type: 'line',

        // The data for our dataset
        data: {
            labels: labels,
            datasets: [{
                label: 'Total Count',
                backgroundColor: 'transparent',
                borderColor: 'steelblue',
                data: totalCount
            },
            {
                label: 'Reject Count',
                backgroundColor: 'transparent',
                borderColor: 'rgb(255, 99, 132)',
                data: rejectCount
            }]
        },

        // Configuration options go here
        options: {
            animation: {
                duration: 0
            }
        }
    });

    let availability = lineChartData.map(a => a.availability);
    let performance = lineChartData.map(a => a.performance);
    let quality = lineChartData.map(a => a.quality);
    let oee = lineChartData.map(a => a.oee);

    let availabilityPercent = availability[availability.length - 1] * 100;
    let performancePercent = performance[performance.length - 1] * 100;
    let qualityPercent = quality[quality.length - 1] * 100;
    let oeePercent = oee[oee.length - 1] * 100;

    availabilitySpan.innerHTML = parseInt(availabilityPercent) + '%';
    performanceSpan.innerHTML = parseInt(performancePercent) + '%';
    qualitySpan.innerHTML = parseInt(qualityPercent) + '%';
    oeeSpan.innerHTML = parseInt(oeePercent) + '%';


    var OEEChart = new Chart(ctxOEE, {
        // The type of chart we want to create
        type: 'bar',

        // The data for our dataset
        data: {
            labels: ['Availability', 'Performance', 'Quality', 'OEE'],
            datasets: [{
                backgroundColor: 'steelblue',
                borderColor: 'steelblue',
                data: [
                    availabilityPercent,
                    performancePercent,
                    qualityPercent,
                    oeePercent
                ]
            }]
        },

        // Configuration options go here
        options: {
            legend: { display: false },
            responsive: true,
            maintainAspectRatio: false,
            animation: {
                duration: 0
            },
            scales: {
                yAxes: [{
                    display: true,
                    ticks: {
                        suggestedMin: 0,    // minimum will be 0, unless there is a lower value.
                        // OR //
                        beginAtZero: true   // minimum value will be 0.
                    }
                }]
            }
        }
    });
});


connection.start().then(function () {
    document.getElementById("startSimulation").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

//document.getElementById("sendButton").addEventListener("click", function (event) {
//    var user = document.getElementById("userInput").value;
//    var message = document.getElementById("messageInput").value;
//    connection.invoke("SendMessage", user, message).catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});

document.getElementById("startSimulation").addEventListener("click", function (event) {
    var shiftLength = parseInt(document.getElementById("shiftLength").value);
    var totalBreakTime = parseInt(document.getElementById("totalBreakTime").value);
    var downtime = parseInt(document.getElementById("downtime").value);
    var radioCycleTime = document.getElementById("radioCycleTime").checked;
    var radioRunRate = document.getElementById("radioRunRate").checked;
    var idealCycleTime = parseFloat(document.getElementById("idealCycleTime").value);
    var idealRunRate = parseInt(document.getElementById("idealRunRate").value);
    var actualCycleTimeMin = parseFloat(document.getElementById("actualCycleTimeMin").value);
    var actualCycleTimeMax = parseFloat(document.getElementById("actualCycleTimeMax").value);
    var actualRunRateMin = parseFloat(document.getElementById("actualRunRateMin").value);
    var actualRunRateMax = parseFloat(document.getElementById("actualRunRateMax").value);
    var rejectCountMin = parseInt(document.getElementById("rejectCountMin").value);
    var rejectCountMax = parseInt(document.getElementById("rejectCountMax").value);
    var simInterval = parseInt(document.getElementById("simInterval").value);

    var formData = {
        shiftLength,
        totalBreakTime,
        downtime,
        radioCycleTime,
        radioRunRate,
        idealCycleTime,
        idealRunRate,
        actualCycleTimeMin,
        actualCycleTimeMax,
        actualRunRateMin,
        actualRunRateMax,
        rejectCountMin,
        rejectCountMax,
        simInterval

    };

    console.log(formData);
    connection.invoke("StartSimulation", formData).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("stopSimulation").addEventListener("click", function (event) {
    console.log('Stop Simulation');
    connection.invoke("StopSimulation").catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});


document.getElementById("radioCycleTime").addEventListener("click", function (event) {
    console.log("Cycle time selected!");
    document.getElementById("idealRunRate").disabled = true;
    document.getElementById("idealCycleTime").disabled = false;
    document.getElementById("actualRunRateMin").disabled = true;
    document.getElementById("actualRunRateMax").disabled = true;
    document.getElementById("actualCycleTimeMin").disabled = false;
    document.getElementById("actualCycleTimeMax").disabled = false;



    //event.preventDefault();
});

document.getElementById("radioRunRate").addEventListener("click", function (event) {
    console.log("Run Rate selected!");
    document.getElementById("idealCycleTime").disabled = true;
    document.getElementById("idealRunRate").disabled = false;
    document.getElementById("actualCycleTimeMin").disabled = true;
    document.getElementById("actualCycleTimeMax").disabled = true;
    document.getElementById("actualRunRateMin").disabled = false;
    document.getElementById("actualRunRateMax").disabled = false;

    //event.preventDefault();
});


