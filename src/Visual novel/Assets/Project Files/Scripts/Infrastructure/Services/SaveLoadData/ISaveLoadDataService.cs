#region

using Data.Dynamic;

#endregion

namespace YG
{
	public interface ISaveLoadDataService
	{
		GameData LoadOrCreateNew();
		void Save(GameData gameData);
		bool Exists();
		void Remove();
	}
}