/// <reference path="../../../../typings/tsd.d.ts" />
/// <reference path="../../../../scripts/Intellitect/intellitect.utilities.ts" />
/// <reference path="../../../../scripts/Intellitect/intellitect.ko.utilities.ts" />

module TestArea.ViewModels {
    // *** External Type PersonStats
    export class PersonStats
    {
        // Observables
		public personStatsId: KnockoutObservable<number> = ko.observable(null);
		public height: KnockoutObservable<number> = ko.observable(null);
		public weight: KnockoutObservable<number> = ko.observable(null);
		public personLocation: KnockoutObservable<ViewModels.PersonLocation> = ko.observable(null);
        // Loads this object from a data transfer object received from the server.
        public loadFromDto: (data: any) => void;
        public parent: any;


        constructor(newItem?: any, parent?: any){
            var self = this;
            self.parent = parent;
            // Load the object
			self.loadFromDto = function(data: any) {
				if (!data) return;

                // Load the properties.
                self.personStatsId(data.PersonStatsId);
                self.height(data.Height);
                self.weight(data.Weight);
                self.personLocation(new PersonLocation());
                self.personLocation().loadFromDto(data.PersonLocation);
                
            };

            if (newItem) {
                self.loadFromDto(newItem);
            }
        }
    }
}