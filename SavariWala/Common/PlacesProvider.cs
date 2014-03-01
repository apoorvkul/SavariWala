using System;
using System.Text;
using System.Net;
using System.Json;
using SavariWala.Common;
using System.Linq;
using System.Collections.Generic;

namespace SavariWala
{

	public class GeoLoc 
	{
		public double Lat { get; set; }
		public double Lng { get; set; }
	}

	public class Place 
	{
		public string Name { get; set; }
		public string Address { get; set; }
		public GeoLoc Loc { get; set; }
	}

	public class PlacesProvider
	{
		// TODO: use component to restrict by country
		private const string AutoCompleteUrlFormat = 
			"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={0}&sensor={1}&key={2}&offset={3}&location={4},{5}";

		// Max. radius allowed by api = 50000m
		private const string SearchUrlFormat = 
			"https://maps.googleapis.com/maps/api/place/textsearch/json?query={0}&sensor={1}&key={2}&location={3},{4}&radius=50000";

		public void AutoCompleteAsync (Action<List<string>> updater, string input, int offset, double latitude, double longitude)
		{
			DownloadStringAsync(r =>
				{
					var jsonObj = (JsonObject) JsonValue.Parse(r);
					var status = (string) jsonObj["status"];
					if(status == "OK") 
					{
						var pred = (JsonArray) jsonObj["predictions"];
						updater(pred.Select(
							x => (string)((JsonObject)x)["description"]).ToList()
						);
					}
					else AppCommon.Inst.Log.Error("Error Fetching Places AutoCompletion: {0}", status); 
				}, string.Format(AutoCompleteUrlFormat, input, "true", AppCommon.Inst.GoogleApiKeyWeb, offset, latitude, longitude));
		}

		public void SearchAsync (Action<List<Place>> updater, string input, double latitude, double longitude)
		{
			input = String.Join ("+", input.Split (new Char[]{' '}, StringSplitOptions.RemoveEmptyEntries));
			DownloadStringAsync(r => {
				var jsonObj = (JsonObject) JsonValue.Parse(r);
				var resList = ((JsonArray) jsonObj["results"]).Select (x => { 
					var sr = (JsonObject)x;
					var loc = ((JsonObject)sr["geometry"])["location"];
					return new Place { Name = (string)sr ["name"], Address = (string)sr ["formatted_address"], 
						Loc = new GeoLoc { Lat = (double)loc["lat"], Lng = (double)loc["lng"] } };
				}).ToList ();
				updater(resList);
			}, string.Format(SearchUrlFormat, input, "true", AppCommon.Inst.GoogleApiKeyWeb, latitude, longitude));
		}

		public void DownloadStringAsync (Action<string> handler, string url)
		{
			var wc = new WebClient();
			wc.DownloadStringCompleted += (s, e) =>
			{
				try
				{
					AppCommon.Inst.Log.Debug("{0}: {1}", url, e.Result);
					handler(e.Result);
				}
				catch (Exception ex)
				{
					AppCommon.Inst.Log.Error("Error Fetching Results from {0}: {1}", url, ex); 
				}
			};

			wc.DownloadStringAsync (new Uri (url));
		}
	}
}

 