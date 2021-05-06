using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LanguageManager : Singleton<LanguageManager> {
	
	public readonly List<Language> Languages = new List<Language>();
	public readonly ValueObservable<Language> CurrentLanguage = new ValueObservable<Language>(null);

	[SerializeField] private OverridesInstaller overridesInstaller; // scriptable object'ы с настройками
	[SerializeField] private ArtInstaller artInstaller;

	protected override void AwakeSingletone()
	{
		TextAsset[] langAssets = Resources.LoadAll<TextAsset>("Lang"); // подгружает из папки ресурсов lang файлы
		foreach (TextAsset asset in langAssets) {
			List<string> lines = Encoding.UTF8.GetString(asset.bytes).Split(new [] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries).ToList();
			lines.RemoveAll(x => x[0].Equals('#') );
			
			// парсинг lang файла, с записью ключей и значений в словарь
			Dictionary<string, string> toLang = new Dictionary<string, string>();
			for (int i = 1; i < lines.Count; i++) {
				string[] splitted = lines[i].Split(new [] {'='}, StringSplitOptions.RemoveEmptyEntries);
				try {
					toLang.Add(splitted[0], splitted[1]);
				}
				catch (IndexOutOfRangeException e) {
					Debug.Log(e);
				}
			}

			string[] languageInfo = lines[0].Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
			Languages.Add(new Language( // регустрирует новый язык со всей информацией о нём
				languageInfo[0], 
				languageInfo[1], 
				(SystemLanguage) int.Parse(languageInfo[2]), 
				artInstaller.LanguageFonts[int.Parse(languageInfo[3])], 
				toLang
			));
		}

		// Чтобы можно было настраивать текущий язык из редактора unity через scriptable object
		if (overridesInstaller.LanguageCode.Equals("system"))
		{
			if (!SetLanguage(lang => lang.System == Application.systemLanguage))
				SetDefaultLanguage();
		}
		else if (!SetLanguage(lang => lang.Code.Equals(overridesInstaller.LanguageCode)))
			SetDefaultLanguage();
	}
	
	public void SetDefaultLanguage() {
		SetLanguage(x => x.Code.Equals("en"));
	}
	
	public bool SetLanguage(Predicate<Language> predicate) {
		Language toCurrent = Languages.Find(predicate);
		
		if (toCurrent != null)
			SetLanguage(toCurrent);
			
		return toCurrent != null;
	}

	public void SetLanguage(Language language) { // можем менять язык вызвав этот метод, и тогда ValueObservable сообщит всем подписчикам об его изменении
		CurrentLanguage.Value = language;
	}
	
	public string GetValue(string key) {
		try {
			return CurrentLanguage.Value.Dictionary[key];
		}
		catch (KeyNotFoundException e) {
			return key; // если по ключу не нашли значение, то возвращаем этот же ключ в качесвте перевода
		}
	}

	public class Language {
		
		public string Name;
		public string Code;
		public SystemLanguage SystemLanguage;
		public TMP_FontAsset Font;
		public Dictionary<string, string> Dictionary; // список ключ=значение для языка
		
		public Language(string name, string code, SystemLanguage systemLanguage, TMP_FontAsset font, Dictionary<string, string> dictionary) {
			Name = name;
			Code = code;
			SystemLanguage = systemLanguage;
			Font = font;
			Dictionary = dictionary;
		}
	}
}
