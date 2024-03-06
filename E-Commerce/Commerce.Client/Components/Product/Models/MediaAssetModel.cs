namespace Commerce.Client;

public sealed class MediaAssetModel
{
    public Guid Id { get; set; }

    public string RelativePath { get; set; }

    public string AbsolutePath { get; set; }

    public string ImagePath => $"http://localhost:7474{AbsolutePath}?sv=2021-10-04&ss=btqf&srt=sco&spr=https%2Chttp&st=1975-12-17T21%3A10%3A00Z&se=2109-12-19T21%3A10%3A00Z&sp=rl&sig=4e12a51r6wGN%2B1rlBCs5T5TL%2FG6UzkYrUYHgban7QmQ%3D";

    public DateTime Timestamp { get; set; }
}