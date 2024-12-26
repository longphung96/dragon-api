using DragonAPI.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace DragonAPI.Common
{
    public class CommonError
    {
        [JsonPropertyName("code")]
        public string Code { get; private set; }
        [JsonPropertyName("message")]
        public string Message { get; private set; }
        public CommonError(ErrorCodeEnum errorCode, string message = null)
        {
            Code = errorCode.ToString();
            Message = message ?? GetDescription(errorCode);
        }

        private string GetDescription(Enum enumValue)
        {
            //Look for DescriptionAttributes on the enum field
            object[] attr = enumValue.GetType().GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attr.Length > 0) // a DescriptionAttribute exists; use it
                return ((DescriptionAttribute)attr[0]).Description;

            return string.Empty;
        }
    }
    public class ApiResultDTO<TData>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("errors")]
        public List<CommonError> Errors { get; set; } = null;
        // [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("data")]
        public TData Data { get; set; }
        public bool IsSucceed
        {
            get
            {
                if (Errors != null && Errors.Count > 0)
                    return false;
                return true;
            }
        }

        public void AddError(ErrorCodeEnum errorCode, string message = null)
        {
            AddError(new CommonError(errorCode, message));
        }
        public void AddError(CommonError error)
        {
            if (Errors == null)
                Errors = new List<CommonError>();
            Errors.Add(error);
        }
        public List<string> GetListErrorString()
        {
            var listErrorString = new List<string>();
            if (IsSucceed)
                return listErrorString;
            foreach (var error in Errors)
            {
                listErrorString.Add(error.Message);
            }
            return listErrorString;
        }
    }
}