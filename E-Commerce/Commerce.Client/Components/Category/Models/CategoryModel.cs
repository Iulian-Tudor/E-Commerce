﻿namespace Commerce.Client;

public sealed class  CategoryModel 
{
	public Guid Id { get; set; } = Guid.Empty;

	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;
}
