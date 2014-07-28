(function () {
    "use strict";
    var dashboard = $.connection.dashboard;

    $.extend(dashboard.client, {
        wei: function (wei) {
            var model = new Wei(wei);
            new WeiView({
                model: model
            }).renderTo(document.querySelector("#timeline"), "prepend");
            var weiwei = $(".wei");
            var index = weiwei.length - 200;
            while (index > 0) {
                $(weiwei[200 + index--]).remove();
            }
        }
        , onechan: function (id) {
            document.querySelector("#" + id).skeleton.onechan();
        }
        , comment: function (comment) {
            var model = new Comment(comment);
            new CommentView({
                model: model
            }).renderTo(document.querySelector("#wei-" + model.WeiId + " .comments"));
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
    
    var Wei = Skeleton.Model.extends({
        constructor:function(props){
            this.base()(props);
            this.domid = "wei-" + this.id;
            this.setDisplayPostDate()
            this.update();
        },
        properties: ["id", "domid", "text", "imageUrl", "userName", "userIcon", "postDate", "displayPostDate", "onechances"]
        , update: function () {
            var that = this;
            setTimeout(function () {
                that.setDisplayPostDate();
                that.update();
            }, 60 * 1000);
        }
        , setDisplayPostDate: function () {;
            var date = new Date(this.postDate)
            var diff = new Date() - date;
            var displayDate = "";
            if (diff < 60 * 1000) {
                displayDate = "さっき";
            }
            else if (diff < 60 * 60 * 1000) {
                var min = ~~(diff / (60 * 1000));
                displayDate = min + "分前";
            }
            else if (diff < 24 * 60 * 60 * 1000) {
                var hour = ~~(diff / (60 * 60 * 1000));
                displayDate = hour + "時間前";
            }
            else if (diff < 7 * 24 * 60 * 60 * 1000) {
                var day = ~~(diff / (24 * 60 * 60 * 1000));
                displayDate = day + "日前";
            }
            else {
                displayDate = date.getFullYear() + "年" + (date.getMonth() + 1) + "月" + date.getDate() + "日";
            }
            this.displayPostDate = displayDate;
        }
    });

    var WeiView = Skeleton.View.extends({
        template: new Skeleton.HTMLTemplate("#wei-template")
        , events: {
            "rendered": "initialize"
            , "click .onechan": "postOnechan"
            , "click .reply": "reply"
            , "click .more": "more"
        }
        , initialize: function () {
            var comment = [], i, len, comments = this.find(".comments");
            if (this.model.comments.length < 5) {
                $(this.find(".more")).hide();
            }
            for (i = 0, len = this.model.comments.length; i < 5 && i < len; i++) {
                comment = this.model.comments[i];
                var model = new Comment(comment);
                new CommentView({
                    model: model
                }).renderTo(comments);
            }
        }
        , postOnechan: function () {
            $.get("/api/onechances/" + this.model.id);
        }
        , onechan: function () {
            this.model.onechances++;
        }
        , reply: function(){
            var messageId = $(this.find(".message-list")).val();
            $.post("/api/comments", {
                MessageId: messageId
                , WeiId: this.model.id
            }).fail(function (response) {
                console.log(response);
            });
        }
        , more: function () {
            var comment = [], i, len, comments = this.find(".comments");
            for (i = 4, len = this.model.comments.length; i < len; i++) {
                comment = this.model.comments[i];
                var model = new Comment(comment);
                new CommentView({
                    model: model
                }).renderTo(comments);
            }
            $(this.find(".more")).hide();
        }
    });
    var Comment = Skeleton.Model.extends({
        constructor: function(props){
            this.base()(props);
            if (!!props.User) {
                this.userName = props.User.UserName;
                this.userIcon = props.User.Icon;
            }
            if(!!props.Message){
                this.text = this.Message.Text;
            }
        }
        , properties: ["Id", "userIcon", "userName", "text", "MessageId"]
    });
    var CommentView = Skeleton.View.extends({
        template: new Skeleton.HTMLTemplate("#comment-template")
    })
    var Application = new (Skeleton.Class.extends({
        load: function () {
            var deffered = new $.Deferred();
            $(function () {
                $("#more").hide();
                deffered.resolve();
            });
            return deffered.promise();
        }
        , start: function () {
            var uploader = new plupload.Uploader({
                browse_button: "image-uploader"
                    , url: "/api/Images"
                    , multi_selection: false
                    , unique_names: true
                    , filters: {
                        mime_types: [{ title: "Image files", extensions: "jpg,gif,png" }]
                    }
                    , resize: {
                        width:600
                    }
                    , runtimes: 'html5'
                    , init: {
                        FilesAdded: function (up, files) {
                            uploader.start();
                        }
                        , FileUploaded: function (up, file, data) {
                            var url = JSON.parse(data.response).Path;
                            $("#image-url").val(url);
                            $("#upload-image").attr("src", url);
                        }
                    }
            });
            uploader.init();
            $("#post-wei").click(function () {
                var data = $("#message-list").val();
                $.post("/api/weiwei", {
                    MessageId: $("#message-list").val()
                    , ImageUrl: $("#image-url").val()
                }).done(function () {
                });
                $("#image-url").val("");
                $("#upload-image").attr("src", "");
            });
            $.get("/api/weiwei").done(function (weiwei) {
                weiwei.forEach(function (wei) {
                    var model = new Wei(wei);
                    new WeiView({
                        model: model
                    }).renderTo(document.querySelector("#more"), "before");
                })
                if (weiwei.length == 20) {
                    $("#more").show();
                }
            })
            $("#more").click(function () {
                var id = $("#more").prev().get(0).skeleton.model.id;
                $.get("/api/weiwei", {
                    before: id
                }).done(function (weiwei) {
                    weiwei.forEach(function (wei) {
                        var model = new Wei(wei);
                        new WeiView({
                            model: model
                        }).renderTo(document.querySelector("#more"), "before");
                    });
                    if (weiwei.length < 20) {
                        $("#more").hide();
                    }
                })
            });
        }
    }))();

    $.when($.connection.hub.start(), Application.load()).done(function () {
        Application.start();
    });
}());