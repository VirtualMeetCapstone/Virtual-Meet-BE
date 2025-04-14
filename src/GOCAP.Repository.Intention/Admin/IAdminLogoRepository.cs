namespace GOCAP.Repository.Intention
{
    public interface IAdminLogoRepository
    {
        Task<AdminLogoEntity> UpdateAsync(AdminLogoEntity entity);
        Task<AdminLogoEntity> GetAsync();
    }
}
