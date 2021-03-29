using Sitecore;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Spe.Core.Host;
using System;
using System.Collections.Generic;

namespace SharedSitecore
{
    public static class ExecuteScript
    {
        public static List<T> ToList<T>(string scriptOrId, Dictionary<string,string> parameters = null, string sessionId = "Default")
        {
            if (parameters == null) parameters = new Dictionary<string, string>();
            using (var scriptSession = ScriptSessionManager.NewSession(sessionId, true))
            {
                if (ID.IsID(scriptOrId))
                {
                    var speScriptItem = Context.Database.GetItem(new ID(scriptOrId));
                    scriptOrId = speScriptItem["Script"];
                }
                if (!string.IsNullOrEmpty(scriptOrId))
                {                    
                    foreach(var parameter in parameters) {
                        try
                        {
                            scriptSession.SetVariable(parameter.Key, parameter.Value);
                        }
                        catch (Exception exception)
                        {
                            Log.Error($"{typeof(ExecuteScript).FullName}.Process({scriptOrId}) ERROR processing parameter - {parameter.Key}:{exception.Message}", exception, typeof(ExecuteScript));
                        }
                    }

                    var scriptResults = scriptSession.ExecuteScriptPart(scriptOrId, false);
                    var results = new List<T>();

                    foreach(var result in scriptResults) {
                        try
                        {
                            results.Add((T)result);
                        }
                        catch (Exception exception)
                        {
                            Log.Error($"{typeof(ExecuteScript).FullName}.Process({scriptOrId}) ERROR processing result - {result}:{exception.Message}", exception, typeof(ExecuteScript));
                        }
                    }
                    return results;
                }
            }
            return null;
        }
    }
}