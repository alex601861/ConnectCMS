(function() {
    window.blazorGetLocationPermission = {
        requestLocationStatus: async () => {
            if (!navigator.geolocation) {
                console.log('Geolocation is not supported for this Browser/OS.');
                return 'denied';
            }

            return navigator.permissions.query({
                name: 'geolocation'
            }).then(function(result) {
                console.log('Getting Location State - ' + result.state);
                return result.state;
            });
        }
    };

    window.blazorGetLocationInformation = {
        requestCurrentLocation: () => {
            var geoInstanceOptions = {
                timeout: 10 * 1000,
                maximumAge: 5 * 60 * 1000,
                enableHighAccuracy: false
            };

            var myPromise = new Promise((resolve, reject) => {
                navigator.geolocation.getCurrentPosition(
                    position => {
                        let geoLocation = {
                            Latitude: position.coords.latitude,
                            Longitude: position.coords.longitude,
                            Accuracy: position.coords.accuracy
                        }
                        resolve(geoLocation);

                    }, error => {
                        reject(error)
                    }, geoInstanceOptions
                )
            }).catch(error => error);
            return myPromise;
        },
    };
    
    window.triggerLocationPrompt = function() {
        if (navigator.geolocation) {
            return new Promise((resolve, reject) => {
                navigator.geolocation.getCurrentPosition(
                    (position) => {
                        resolve(true); 
                    },
                    (error) => {
                        resolve(false); 
                    }
                );
            });
        } else {
            console.log('Geolocation is not supported by this Browser/OS.');
            return Promise.resolve(false);
        }
    };
})();