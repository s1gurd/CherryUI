using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Scripting;

namespace CherryUI.DataModels
{
	[Preserve]
	public class ModelService : IDisposable
	{
		private readonly Dictionary<Type, DataModelBase> _singletonModels = new();
		private readonly List<DataModelBase> _playerPrefsSingletonModels = new();

        public T GetOrCreateSingletonModel<T>() where T : DataModelBase
        {
            if (_singletonModels.TryGetValue(typeof(T) , out var model))
            {
                return model as T;
            }
            
            var newModel = Activator.CreateInstance<T>();
            _singletonModels[typeof(T)] = newModel;
            return newModel;
        }

		public void LinkModelToPlayerPrefs<T>(T model) where T : DataModelBase
		{
			Debug.Log($"[Model Service - PlayerPrefs] Linking model {typeof(T)} to Player Prefs...");
			var key = typeof(T).ToString();
			if (PlayerPrefs.HasKey(key))
			{
				var json = PlayerPrefs.GetString(key);
				JsonConvert.PopulateObject(json, model);
				Debug.Log($"[Model Service - PlayerPrefs] Got model {typeof(T)} data from PlayerPrefs: {json}");
			}

			model.Ready = true;

			if (_singletonModels.TryGetValue(typeof(T), out var instance) && instance == model)
			{
				_playerPrefsSingletonModels.Add(model);
				Debug.Log($"[Model Service - PlayerPrefs] Model {typeof(T)} is Singleton and will be saved upon exit automatically");
			}
			else
			{
				Debug.Log($"[Model Service - PlayerPrefs] Model {typeof(T)} is Transient and will NOT be saved automatically. Consider saving it by hand");
			}
		}

		public void SavePlayerPrefsModel(DataModelBase model)
		{
			var key = model.GetType().ToString();
			var json = JsonConvert.SerializeObject(model);
			PlayerPrefs.SetString(key, json);
			Debug.Log($"[Model Service - PlayerPrefs] Saved model of type {key} with content: {json}");
		}

		public void SaveAllSingletonPlayerPrefsModels()
		{
			foreach (var model in _playerPrefsSingletonModels)
			{
				SavePlayerPrefsModel(model);
			}
			PlayerPrefs.Save();
		}

		public void Dispose()
		{
			SaveAllSingletonPlayerPrefsModels();
		}
	}
}