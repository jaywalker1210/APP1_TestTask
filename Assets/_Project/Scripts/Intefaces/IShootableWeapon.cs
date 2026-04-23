using Assets._Project.Scripts.Data;

namespace Assets._Project.Scripts.Intefaces
{
    public interface IShootableWeapon
    {
        public int Damage { get; }
        public AmmoData[] CompatibleAmmoIds { get; } // список подходящих патронов
    }
}
