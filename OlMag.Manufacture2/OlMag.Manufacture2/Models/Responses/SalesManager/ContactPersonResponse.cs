﻿using System.ComponentModel.DataAnnotations;

namespace OlMag.Manufacture2.Models.Responses.SalesManager;

public class ContactPersonResponse
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ФИО
    /// </summary>
    [Required]
    public string Name { get; set; } = default!;

    /// <summary>
    /// Должность
    /// </summary>
    [Required]
    public string Post { get; set; } = string.Empty;

    /// <summary>
    /// Телефон
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Почта
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Примечание
    /// </summary>
    public string? Note { get; set; }
}