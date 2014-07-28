(function () {
    "use strict";
    var dashboard = $.connection.dashboard;

    $.extend(dashboard.client, {
        wei: function (wei) {
        }
        , onechan: function (id) {
        }
        , comment: function (comment) {
        }
    });

    $.connection.hub.stateChanged(function (change) {
        var oldState = null,
            newState = null;

        for (var state in $.signalR.connectionState) {
            if ($.signalR.connectionState[state] === change.oldState) {
                oldState = state;
            } else if ($.signalR.connectionState[state] === change.newState) {
                newState = state;
            }
        }
        console.log("[" + new Date().toTimeString() + "]: " + oldState + " => " + newState + " " + $.connection.hub.id);
    });
    $.connection.hub.reconnected(function () {
        console.log("[" + new Date().toTimeString() + "]: Connection re-established");
        console.log("reconnected transport: " + $.connection.hub.transport.name + " " + $.connection.hub.id);
    });
    $.connection.hub.error(function (err) {
        console.log("Error occurred: " + err);
    });
    $.connection.hub.connectionSlow(function () {
        console.log("[" + new Date().toTimeString() + "]: Connection Slow");
    });
    $.connection.hub.disconnected(function () {
        console.log("disconnected!");
    });

    var Message = Skeleton.Model.extends({
        properties: ["Id", "Text"]
    })

    var MessageView = Skeleton.View.extends({
        template: new Skeleton.HTMLTemplate("#message-template")
        , events: {
            "click .buy": "buy"
        }
        , buy: function () {
            $.post("/api/messages", this.model.export()).done(this.delegate(function () {
                this.find(".buy").style.display = "none";
            }))
        }
    })

    var Application = new (Skeleton.Class.extends({
        load: function () {
            var deffered = new $.Deferred();
            $(function () {
                //$("#more").hide();
                deffered.resolve();
            });
            return deffered.promise();
        }
        , start: function () {
            $.get("/api/messages").done(function (response) {
                var messages = document.querySelector("#messages");
                response.forEach(function (data) {
                    var model = new Message(data);
                    new MessageView({
                        model: model
                    }).renderTo(messages)
                });
            })
        }
    }))();

    $.when($.connection.hub.start(), Application.load()).done(function () {
        Application.start();
    });
}())