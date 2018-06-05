(function($){
            /*Bootstrap确认对话框*/
            function BootstrapConfirm(selector){
                this._selector = selector;
                this._cancelCallback = null;
                this._okCallback = null;
            }
            BootstrapConfirm.prototype.init = function(fn_cancel, fn_ok){
                var cancel = $("#btnCancel");
                var self = this;
                this._cancelCallback = fn_cancel;
                this._okCallback = fn_ok;
                if(cancel){
                    cancel.unbind('click').click(function(){
                        self._result = false;
                        if(self._cancelCallback && typeof(self._cancelCallback) == "function"){
                            self._cancelCallback.call(self);
                        }
                    });
                }
                var confirm = $("#btnConfirm");
                if(confirm){
                    confirm.unbind('click').click(function(){
                        self._result = true;
                        $(self._selector).modal('hide');
                        if(self._okCallback && typeof(self._okCallback) == "function"){
                            self._okCallback.call(self);
                        }
                    });
                }
            }
            BootstrapConfirm.prototype.confirm = function(title, message){
                if(title && title != ""){
                    $("#confirmTitle").text(title);
                }
                if(title && title != ""){
                    $("#confirmBody").html('<p class="text-warning">' + message +'</p>');
                }
                $(this._selector).modal('show');
            }
            /*Bootstrap模态对话框*/
            function BootstrapModal(selector){
                this._selector = selector;
                this._initCallback = null;
                this._okCallback = null;
            }
            BootstrapModal.prototype.init = function(title, fn_init, fn_ok){
                var self = this;
                this._initCallback = fn_init;
                this._okCallback = fn_ok;
                if(title && typeof(title) == "string"){
                    $("#titleModal").text(title);
                }
                $("#btnModalSubmit").unbind('click').click(function(){
                    //$(self._selector).modal('hide');
                    if(self._okCallback && typeof(self._okCallback) == "function"){
                        self._okCallback.call(self);
                    }
                });
            }
            BootstrapModal.prototype.modal = function(){
                if(this._initCallback && typeof(this._initCallback) == "function"){
                    this._initCallback.call(this);
                }
                $(this._selector).modal('show');
            }
            /*Bootstrap消息框*/
            function BootstrapMessage(selector){
                this._selector = selector;
            }
            BootstrapMessage.prototype.message = function(title, message){
                if(title && title != ""){
                    $("#infoTitle").text(title);
                }
                if(title && title != ""){
                    $("#infoBody").html('<p class="text-primary">' + message +'</p>');
                }
                $(this._selector).modal('show');
            }

            window.bootstrapModal = {};
            window.bootstrapModal.confirm = BootstrapConfirm;
            window.bootstrapModal.message = BootstrapMessage;
            window.bootstrapModal.modal = BootstrapModal;
        }(jQuery))