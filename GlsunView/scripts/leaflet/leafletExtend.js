(function(){
    /*扩展一个工具条控件 */
    L.Control.Toolbar = L.Control.extend({
        onAdd: function(map) {
            if(!this.options.elemId || this.options.elemId == ""){
                throw new Error('L.Control.Toolbar initialized failed.The elemId is missed.');
            }
            var toolbar = document.getElementById(this.options.elemId);
            return toolbar;
        },
        
        onRemove: function(map) {
            // Nothing to do here
        }
    });
    L.control.toolbar = function(opts){
        return new L.Control.Toolbar(opts);
    }

    /*扩展标记控件*/
    L.Marker.LineNode = L.Marker.extend({
        onAdd: function(map){
            var nodeIcon = L.divIcon({className: 'marker-icon-node'});
            this.options.icon = nodeIcon;
            L.Marker.prototype.onAdd.call(this, map);
        }
    });
    //工厂方法
    L.marker.lineNode = function(latlng, opts){
        return new L.Marker.LineNode(latlng, opts);
    }

    //多线段添加新方法
    L.Polyline.include({
        //根据距离获取经纬度
        getLatlngByDistance: function(distance){
            if (!this._map) {
                throw new Error('Must add layer to map before using getCenter()');
            }

            var i, halfDist, segDist, dist, p1, p2, ratio, pixelLength, realLength
            points = this._rings[0],
            len = points.length;

            //计算像素总长和真实总长
            for (i = 0, pixelLength = 0, realLength = 0; i < len - 1; i++) {
                p1 = points[i];
                p2 = points[i + 1];
                segDist = p1.distanceTo(p2);
                pixelLength += segDist; 

                p1 = this._latlngs[i];
                p2 = this._latlngs[i + 1];
                segDist = p1.distanceTo(p2);
                realLength += segDist; 
            }
            if(distance === realLength){
                return this._map.layerPointToLatLng(points[len - 1]);
            }
            if(distance > realLength){
                return null;
            }
            //距离转换为像素
            pixelDist = Math.round(distance / realLength * pixelLength);
            for (i = 0, dist = 0; i < len - 1; i++) {
                p1 = points[i];
                p2 = points[i + 1];
                segDist = p1.distanceTo(p2);
                dist += segDist; 

                if (dist > pixelDist) {
                    ratio = (dist - pixelDist) / segDist;
                    return this._map.layerPointToLatLng([
                        p2.x - ratio * (p2.x - p1.x),
                        p2.y - ratio * (p2.y - p1.y)
                    ]);
                }
            }
        }
    });
})();