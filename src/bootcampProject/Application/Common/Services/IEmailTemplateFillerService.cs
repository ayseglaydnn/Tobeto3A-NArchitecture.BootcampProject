﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Services;

public interface IEmailTemplateFillerService
{
    string PopulateInstructorApplicationApproveEmail(string firstname, string lastname, string email, string password);
}
