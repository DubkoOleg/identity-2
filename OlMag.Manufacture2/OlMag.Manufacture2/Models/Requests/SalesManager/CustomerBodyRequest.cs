﻿using System.ComponentModel.DataAnnotations;

namespace OlMag.Manufacture2.Models.Requests.SalesManager;

public class CustomerBodyRequest
{
    /// <summary>
    /// Наименование организации
    /// </summary>
    [Required]
    public string Name { get; set; } = default!;

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