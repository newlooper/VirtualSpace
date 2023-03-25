namespace Updater;

// https://gist.github.com/dalexsoto/9fd3c5bdbe9f61a717d47c5843384d11

public static class HttpClientProgressExtensions
{
    public static async Task DownloadDataAsync( this HttpClient client,
        string                                                  requestUrl,
        Stream                                                  destination,
        IProgress<float>                                        progress          = null,
        CancellationToken                                       cancellationToken = default )
    {
        using var       response       = await client.GetAsync( requestUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken );
        var             contentLength  = response.Content.Headers.ContentLength;
        await using var downloadStream = await response.Content.ReadAsStreamAsync( cancellationToken );

        // no progress... no contentLength... very sad
        if ( progress is null || !contentLength.HasValue )
        {
            await downloadStream.CopyToAsync( destination, cancellationToken );
            return;
        }

        // Such progress and contentLength much reporting!
        float GetProgressPercentage( float read, float total )
        {
            return read / total * 100f;
        }

        var progressWrapper = new Progress<long>( bytes => progress.Report( GetProgressPercentage( bytes, contentLength.Value ) ) );
        await downloadStream.CopyToAsync( destination, 81920, progressWrapper, cancellationToken );
    }

    private static async Task CopyToAsync( this Stream source,
        Stream                                         destination,
        int                                            bufferSize,
        IProgress<long>                                progress          = null,
        CancellationToken                              cancellationToken = default )
    {
        if ( bufferSize < 0 )
            throw new ArgumentOutOfRangeException( nameof( bufferSize ) );
        if ( source is null )
            throw new ArgumentNullException( nameof( source ) );
        if ( !source.CanRead )
            throw new InvalidOperationException( $"'{nameof( source )}' is not readable." );
        if ( destination == null )
            throw new ArgumentNullException( nameof( destination ) );
        if ( !destination.CanWrite )
            throw new InvalidOperationException( $"'{nameof( destination )}' is not writable." );

        var  buffer         = new byte[bufferSize];
        long totalBytesRead = 0;
        int  bytesRead;
        while ( ( bytesRead = await source.ReadAsync( buffer, cancellationToken ).ConfigureAwait( false ) ) != 0 )
        {
            await destination.WriteAsync( buffer.AsMemory( 0, bytesRead ), cancellationToken ).ConfigureAwait( false );
            totalBytesRead += bytesRead;
            progress?.Report( totalBytesRead );
        }
    }
}