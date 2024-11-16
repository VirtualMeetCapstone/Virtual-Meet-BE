using AutoMapper;

namespace GOCAP.Repository;

[RegisterService(typeof(IMediaRepository))]
internal class MediaRepository(GoCapMsSqlDbContext _context, IMapper _mapper) : RepositoryBase<Media, MediaEntity>(_context, _mapper), IMediaRepository
{
    public async Task<IEnumerable<Media>> GetByPostIdAsync(Post post)
    {
        if (post.Id == Guid.Empty)
        {
            return [];
        }
        var entities =  await _context.Medias.Where(m => m.PostId == post.Id).ToListAsync();
        return _mapper.Map<IEnumerable<Media>>(entities);
    }
}
