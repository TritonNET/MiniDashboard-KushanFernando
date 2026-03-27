using MiniDashboard.Common;
using System.Text.Json;

namespace MiniDashboard.DataAccess
{
    public abstract class BaseStore
    {
        protected readonly SemaphoreSlim m_lock = new(1, 1);

        private bool m_loaded = false;

        protected readonly ILogger m_logger;

        protected readonly IConfigProvider m_configProvider;

        protected BaseStore(ILogger logger, IConfigProvider configProvider)
        {
            m_logger = logger;
            m_configProvider = configProvider;
        }

        protected async Task EnsureLoadedAsync(CancellationToken cancellationToken)
        {
            if (m_loaded)
                return;

            await m_lock.WaitAsync(cancellationToken);
            try
            {
                if (m_loaded)
                    return;

                var filePath = m_configProvider.GetProductStoreFilePath();

                if (!File.Exists(filePath))
                {
                    m_logger.Info("Product store file not found, starting with empty store.", filePath);
                    m_loaded = true;
                    return;
                }

                try
                {
                    await using var stream = new FileStream(
                        filePath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read,
                        bufferSize: 4096,
                        useAsync: true);

                    m_loaded = await LoadAsync(stream, cancellationToken);
                }
                catch (Exception ex)
                {
                    m_logger.Error("Failed to load JSON product store.", ex);
                    throw;
                }
            }
            finally
            {
                m_lock.Release();
            }
        }

        /// <summary>
        /// Assume already in a locked context
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task SaveAsync(CancellationToken cancellationToken)
        {
            try
            {
                var filePath = m_configProvider.GetProductStoreFilePath();

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                await using var jsonStream = await SerializeAsync(cancellationToken);

                await using var fileStream = new FileStream(
                    filePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 8192,
                    useAsync: true);

                await jsonStream.CopyToAsync(fileStream, cancellationToken);

                m_logger.Verbose("Store saved.", filePath);
            }
            catch (Exception ex)
            {
                m_logger.Error("Failed to save JSON Store.", ex);
                throw;
            }
        }

        protected abstract Task<bool> LoadAsync(Stream jsonStream, CancellationToken cancellationToken);

        protected abstract Task<Stream> SerializeAsync(CancellationToken cancellationToken);
    }
}
