// اسکریپت‌های سفارشی برای سیستم مدیریت مدارک

$(document).ready(function() {
    // فعال‌سازی تولتیپ‌ها
    initializeTooltips();
    
    // فعال‌سازی پاپ‌آپ‌ها
    initializePopovers();
    
    // فعال‌سازی اعتبارسنجی فرم‌ها
    initializeFormValidation();
    
    // فعال‌سازی جداول DataTable
    initializeDataTables();
    
    // فعال‌سازی آپلود فایل
    initializeFileUpload();
    
    // فعال‌سازی جستجو و فیلتر
    initializeSearchAndFilter();
    
    // فعال‌سازی انیمیشن‌ها
    initializeAnimations();
});

// فعال‌سازی تولتیپ‌ها
function initializeTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl, {
            placement: 'top',
            delay: { show: 500, hide: 100 }
        });
    });
}

// فعال‌سازی پاپ‌آپ‌ها
function initializePopovers() {
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });
}

// فعال‌سازی اعتبارسنجی فرم‌ها
function initializeFormValidation() {
    const forms = document.querySelectorAll('.needs-validation');
    Array.from(forms).forEach(form => {
        form.addEventListener('submit', event => {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
                
                // نمایش اولین فیلد نامعتبر
                const firstInvalid = form.querySelector(':invalid');
                if (firstInvalid) {
                    firstInvalid.focus();
                    firstInvalid.scrollIntoView({ behavior: 'smooth', block: 'center' });
                }
            }
            form.classList.add('was-validated');
        }, false);
    });
}

// فعال‌سازی جداول DataTable
function initializeDataTables() {
    if ($.fn.dataTable) {
        // تنظیمات پیش‌فرض برای DataTable
        $.extend(true, $.fn.dataTable.defaults, {
            language: {
                url: '/lib/datatables/fa.json'
            },
            responsive: true,
            pageLength: 10,
            lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "همه"]],
            dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>' +
                 '<"row"<"col-sm-12"tr>>' +
                 '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
            drawCallback: function() {
                // فعال‌سازی مجدد تولتیپ‌ها پس از رسم جدول
                initializeTooltips();
            }
        });
    }
}

// فعال‌سازی آپلود فایل
function initializeFileUpload() {
    // مدیریت آپلود فایل با کشیدن و رها کردن
    $('.upload-area').each(function() {
        const uploadArea = $(this);
        const fileInput = uploadArea.find('input[type="file"]');
        
        // کشیدن و رها کردن فایل
        uploadArea.on('dragover', function(e) {
            e.preventDefault();
            e.stopPropagation();
            $(this).addClass('border-success bg-light');
        });
        
        uploadArea.on('dragleave', function(e) {
            e.preventDefault();
            e.stopPropagation();
            $(this).removeClass('border-success bg-light');
        });
        
        uploadArea.on('drop', function(e) {
            e.preventDefault();
            e.stopPropagation();
            $(this).removeClass('border-success bg-light');
            
            const files = e.originalEvent.dataTransfer.files;
            if (files.length > 0) {
                fileInput[0].files = files;
                fileInput.trigger('change');
            }
        });
        
        // کلیک برای انتخاب فایل
        uploadArea.on('click', function() {
            fileInput.click();
        });
        
        // تغییر استایل هنگام انتخاب فایل
        fileInput.on('change', function() {
            if (this.files.length > 0) {
                uploadArea.addClass('border-success bg-light').removeClass('border-primary');
                
                // نمایش نام فایل
                const fileName = this.files[0].name;
                uploadArea.find('.upload-text').text(`فایل انتخاب شده: ${fileName}`);
            } else {
                uploadArea.removeClass('border-success bg-light').addClass('border-primary');
                uploadArea.find('.upload-text').text('فایل خود را اینجا بکشید یا کلیک کنید');
            }
        });
    });
}

// فعال‌سازی جستجو و فیلتر
function initializeSearchAndFilter() {
    // جستجوی زنده
    $('.search-input').on('keyup', function() {
        const searchTerm = $(this).val().toLowerCase();
        const table = $(this).closest('.card').find('table').DataTable();
        
        if (table) {
            table.search(searchTerm).draw();
        }
    });
    
    // فیلتر وضعیت
    $('.status-filter').on('change', function() {
        const status = $(this).val();
        const table = $(this).closest('.card').find('table').DataTable();
        
        if (table) {
            if (status === '') {
                table.column(5).search('').draw();
            } else {
                table.column(5).search(status).draw();
            }
        }
    });
    
    // فیلتر دپارتمان
    $('.department-filter').on('change', function() {
        const department = $(this).val();
        const table = $(this).closest('.card').find('table').DataTable();
        
        if (table) {
            if (department === '') {
                table.column(3).search('').draw();
            } else {
                table.column(3).search(department).draw();
            }
        }
    });
    
    // بازنشانی فیلترها
    $('.reset-filters').on('click', function() {
        const card = $(this).closest('.card');
        card.find('.search-input').val('');
        card.find('.status-filter').val('');
        card.find('.department-filter').val('');
        
        const table = card.find('table').DataTable();
        if (table) {
            table.search('').columns().search('').draw();
        }
    });
}

// فعال‌سازی انیمیشن‌ها
function initializeAnimations() {
    // انیمیشن ورود کارت‌ها
    $('.fade-in').each(function(index) {
        $(this).css('animation-delay', (index * 0.1) + 's');
    });
    
    // انیمیشن hover برای دکمه‌ها
    $('.btn').hover(
        function() {
            $(this).addClass('shadow-sm');
        },
        function() {
            $(this).removeClass('shadow-sm');
        }
    );
    
    // انیمیشن hover برای کارت‌ها
    $('.card').hover(
        function() {
            $(this).addClass('shadow-lg').removeClass('shadow-sm');
        },
        function() {
            $(this).addClass('shadow-sm').removeClass('shadow-lg');
        }
    );
}

// تابع نمایش اعلان‌ها
function showNotification(message, type = 'info', duration = 5000) {
    const alertClass = {
        'success': 'alert-success',
        'error': 'alert-danger',
        'warning': 'alert-warning',
        'info': 'alert-info'
    }[type] || 'alert-info';
    
    const iconClass = {
        'success': 'bi-check-circle-fill',
        'error': 'bi-exclamation-triangle-fill',
        'warning': 'bi-exclamation-triangle-fill',
        'info': 'bi-info-circle-fill'
    }[type] || 'bi-info-circle-fill';
    
    const notification = $(`
        <div class="alert ${alertClass} alert-dismissible fade show position-fixed" 
             style="top: 20px; right: 20px; z-index: 9999; min-width: 300px;" role="alert">
            <i class="${iconClass} me-2"></i>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `);
    
    $('body').append(notification);
    
    // حذف خودکار پس از مدت مشخص
    setTimeout(() => {
        notification.alert('close');
    }, duration);
}

// تابع تأیید حذف
function confirmDelete(title, callback) {
    Swal.fire({
        title: 'تأیید حذف',
        text: `آیا از حذف "${title}" اطمینان دارید؟`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'بله، حذف کن',
        cancelButtonText: 'انصراف',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            callback();
        }
    });
}

// تابع نمایش بارگذاری
function showLoading(element) {
    const $element = $(element);
    $element.prop('disabled', true);
    $element.html('<span class="loading me-2"></span>در حال پردازش...');
}

// تابع مخفی کردن بارگذاری
function hideLoading(element, originalText) {
    const $element = $(element);
    $element.prop('disabled', false);
    $element.html(originalText);
}

// تابع فرمت کردن تاریخ
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('fa-IR', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    });
}

// تابع فرمت کردن حجم فایل
function formatFileSize(bytes) {
    if (bytes === 0) return '0 بایت';
    
    const k = 1024;
    const sizes = ['بایت', 'کیلوبایت', 'مگابایت', 'گیگابایت'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

// تابع کپی کردن متن
function copyToClipboard(text) {
    navigator.clipboard.writeText(text).then(() => {
        showNotification('متن کپی شد', 'success', 2000);
    }).catch(() => {
        showNotification('خطا در کپی کردن متن', 'error');
    });
}

// تابع جستجوی زنده در جداول
function initializeLiveSearch() {
    $('.live-search').on('input', function() {
        const searchTerm = $(this).val().toLowerCase();
        const table = $(this).data('table');
        
        $(table + ' tbody tr').each(function() {
            const rowText = $(this).text().toLowerCase();
            if (rowText.includes(searchTerm)) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    });
}

// تابع به‌روزرسانی شمارنده
function updateCounter(element, count) {
    const $element = $(element);
    const currentCount = parseInt($element.text()) || 0;
    
    $element.text(count);
    
    if (count > currentCount) {
        $element.addClass('animate__animated animate__pulse');
        setTimeout(() => {
            $element.removeClass('animate__animated animate__pulse');
        }, 1000);
    }
}

// تابع مدیریت تم
function toggleTheme() {
    const currentTheme = localStorage.getItem('theme') || 'light';
    const newTheme = currentTheme === 'light' ? 'dark' : 'light';
    
    document.documentElement.classList.remove('light-theme', 'dark-theme');
    document.documentElement.classList.add(`${newTheme}-theme`);
    localStorage.setItem('theme', newTheme);
    
    // به‌روزرسانی آیکون
    const themeToggle = document.getElementById('theme-toggle');
    if (themeToggle) {
        const darkIcon = themeToggle.querySelector('.theme-icon-dark');
        const lightIcon = themeToggle.querySelector('.theme-icon-light');
        
        if (newTheme === 'dark') {
            darkIcon.style.display = 'none';
            lightIcon.style.display = 'inline-block';
        } else {
            darkIcon.style.display = 'inline-block';
            lightIcon.style.display = 'none';
        }
    }
}

// تابع اعمال تم ذخیره شده
function applySavedTheme() {
    const savedTheme = localStorage.getItem('theme') || 'light';
    document.documentElement.classList.remove('light-theme', 'dark-theme');
    document.documentElement.classList.add(`${savedTheme}-theme`);
    
    // به‌روزرسانی آیکون
    const themeToggle = document.getElementById('theme-toggle');
    if (themeToggle) {
        const darkIcon = themeToggle.querySelector('.theme-icon-dark');
        const lightIcon = themeToggle.querySelector('.theme-icon-light');
        
        if (savedTheme === 'dark') {
            darkIcon.style.display = 'none';
            lightIcon.style.display = 'inline-block';
        } else {
            darkIcon.style.display = 'inline-block';
            lightIcon.style.display = 'none';
        }
    }
}

// اعمال تم ذخیره شده هنگام بارگذاری صفحه
applySavedTheme();

// فعال‌سازی دکمه تغییر تم
$('#theme-toggle').on('click', toggleTheme);