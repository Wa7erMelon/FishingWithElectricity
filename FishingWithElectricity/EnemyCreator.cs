using System.ComponentModel;

namespace FishingWithElectricity
{
	public class EnemyCreator
	{
		public EnemyCreator(ModBehaviour modBehaviour)
		{
			modBehaviour.PropertyChanged += ModBehaviour_PropertyChanged;
		}

		private void ModBehaviour_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			throw new System.NotImplementedException();
		}
	}
}