
/// <reference path="../coalesce.dependencies.d.ts" />

// Knockout List View Model for: IWeatherService
// Generated by IntelliTect.Coalesce

module Services {
    export class WeatherServiceClient {
        public readonly apiController: string = "/WeatherService";

        public static coalesceConfig = new Coalesce.ServiceClientConfiguration<WeatherServiceClient>(Coalesce.GlobalConfiguration.serviceClient);
        public coalesceConfig: Coalesce.ServiceClientConfiguration<WeatherServiceClient>
            = new Coalesce.ServiceClientConfiguration<WeatherServiceClient>(WeatherServiceClient.coalesceConfig);

        
        /**
            Methods and properties for invoking server method GetWeather.
        */
        public readonly getWeather = new WeatherServiceClient.GetWeather(this);
        public static GetWeather = class GetWeather extends Coalesce.ClientMethod<WeatherServiceClient, ViewModels.WeatherData> {
            public readonly name = 'GetWeather';
            public readonly verb = 'POST';
            
            /** Calls server method (GetWeather) with the given arguments */
            public invoke = (location: ViewModels.Location, dateTime: moment.Moment, callback: (result: ViewModels.WeatherData) => void = null): JQueryPromise<any> => {
                return this.invokeWithData({ location: location ? location.saveToDto() : null, dateTime: dateTime ? dateTime.format() : null }, callback);
            };
            
            /** Object that can be easily bound to fields to allow data entry for the method's parameters */
            public args = new GetWeather.Args(); 
            public static Args = class Args {
                public location: KnockoutObservable<ViewModels.Location> = ko.observable(null);
                public dateTime: KnockoutObservable<moment.Moment> = ko.observable(null);
            };
            
            /** Calls server method (GetWeather) with an instance of GetWeather.Args, or the value of this.args if not specified. */
            public invokeWithArgs = (args = this.args, callback: (result: ViewModels.WeatherData) => void = null): JQueryPromise<any> => {
                return this.invoke(args.location(), args.dateTime(), callback);
            }
            
            /** Invokes the method after displaying a browser-native prompt for each argument. */
            public invokeWithPrompts = (callback: (result: ViewModels.WeatherData) => void = null): JQueryPromise<any> => {
                var $promptVal: string = null;
                $promptVal = prompt('Date Time');
                if ($promptVal === null) return;
                var dateTime: moment.Moment = moment($promptVal);
                var location: ViewModels.Location = null;
                return this.invoke(location, dateTime, callback);
            };
            
            protected loadResponse = (data: any, callback: (result: ViewModels.WeatherData) => void = null) => {
                if (!this.result()) {
                    this.result(new ViewModels.WeatherData(data.object));
                } else {
                    this.result().loadFromDto(data);
                }
                if (typeof(callback) == 'function')
                    callback(this.result());
            };
        };
    }
}