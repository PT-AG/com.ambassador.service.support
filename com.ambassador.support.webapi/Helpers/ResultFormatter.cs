using Com.Moonlay.NetCore.Lib.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace com.ambassador.support.webapi.Helpers
{
    public class ResultFormatter
    {
        public Dictionary<string, object> Result { get; set; }

        public ResultFormatter(string ApiVersion, int StatusCode, string Message)
        {
            Result = new Dictionary<string, object>();
            AddResponseInformation(Result, ApiVersion, StatusCode, Message);
        }

        public Dictionary<string, object> Ok()
        {
            return Result;
        }

        public Dictionary<string, object> Ok<TModel>(List<TModel> Data, int Page, int Size, int TotalData, int TotalPageData, Dictionary<string, string> Order, List<string> Select)
        {
            Dictionary<string, object> Info = new Dictionary<string, object>
            {
                { "count", TotalPageData },
                { "page", Page },
                { "size", Size },
                { "total", TotalData },
                { "order", Order }
            };

            if (Select.Count > 0)
            {
                var DataObj = Data.AsQueryable().Select(string.Concat("new(", string.Join(",", Select), ")"));
                Result.Add("data", DataObj);
                Info.Add("select", Select);
            }
            else
            {
                Result.Add("data", Data);
            }

            Result.Add("info", Info);

            return Result;
        }

        public Dictionary<string, object> Ok<TModel, TViewModel>(List<TModel> Data, Func<TModel, TViewModel> MapToViewModel)
        {
            List<TViewModel> DataVM = new List<TViewModel>();
            foreach (TModel d in Data)
            {
                DataVM.Add(MapToViewModel(d));
            }

            Result.Add("data", DataVM);

            return Result;
        }

        public Dictionary<string, object> Ok<TModel, TViewModel>(List<TModel> Data, Func<TModel, TViewModel> MapToViewModel, int Page, int Size, int TotalData, int TotalPageData, Dictionary<string, string> Order, List<string> Select)
        {
            Dictionary<string, object> Info = new Dictionary<string, object>
            {
                { "count", TotalPageData },
                { "page", Page },
                { "size", Size },
                { "total", TotalData },
                { "order", Order }
            };

            List<TViewModel> DataVM = new List<TViewModel>();

            foreach (TModel d in Data)
            {
                DataVM.Add(MapToViewModel(d));
            }

            if (Select.Count > 0)
            {
                var DataObj = DataVM.AsQueryable().Select(string.Concat("new(", string.Join(",", Select), ")"));
                Result.Add("data", DataObj);
                Info.Add("select", Select);
            }
            else
            {
                Result.Add("data", DataVM);
            }

            Result.Add("info", Info);

            return Result;
        }

        public Dictionary<string, object> Ok<TModel>(TModel Data)
        {
            Result.Add("data", Data);

            return Result;
        }

        public Dictionary<string, object> Ok<TModel, TViewModel>(TModel Data, Func<TModel, TViewModel> MapToViewModel)
        {
            Result.Add("data", MapToViewModel(Data));

            return Result;
        }

        public Dictionary<string, object> Fail(string Error)
        {
            Result.Add("error", Error);
            return Result;
        }

        public Dictionary<string, object> Fail()
        {
            Result.Add("error", "Internal Server Error");
            return Result;
        }

        public Dictionary<string, object> Fail(ServiceValidationExeption e)
        {
            Dictionary<string, object> Errors = new Dictionary<string, object>();

            foreach (ValidationResult error in e.ValidationResults)
            {
                string key = error.MemberNames.First();

                try
                {
                    Errors.Add(error.MemberNames.First(), JsonConvert.DeserializeObject(error.ErrorMessage));
                }
                catch (Exception)
                {
                    Errors.Add(error.MemberNames.First(), error.ErrorMessage);
                }
            }

            Result.Add("error", Errors);
            return Result;
        }

        public void AddResponseInformation(Dictionary<string, object> Result, string ApiVersion, int StatusCode, string Message)
        {
            Result.Add("apiVersion", ApiVersion);
            Result.Add("statusCode", StatusCode);
            Result.Add("message", Message);
        }
    }
}
