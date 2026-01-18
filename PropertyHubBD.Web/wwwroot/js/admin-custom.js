// Custom JavaScript for PropertyHubBD Admin Dashboard

$(document).ready(function () {
    // Initialize DataTables if present
    if ($.fn.DataTable) {
        $('.data-table').DataTable({
            "responsive": true,
            "lengthChange": true,
            "autoWidth": false,
            "pageLength": 25,
            "order": [[0, "desc"]],
            "language": {
                "search": "Search:",
                "lengthMenu": "Show _MENU_ entries",
                "info": "Showing _START_ to _END_ of _TOTAL_ entries",
                "paginate": {
                    "first": "First",
                    "last": "Last",
                    "next": "Next",
                    "previous": "Previous"
                }
            }
        });
    }

    // Confirmation dialogs for delete actions
    $('form[data-confirm]').on('submit', function (e) {
        const message = $(this).data('confirm');
        if (!confirm(message)) {
            e.preventDefault();
            return false;
        }
    });

    // AJAX form submission with loading state
    $('.ajax-form').on('submit', function (e) {
        e.preventDefault();
        const form = $(this);
        const button = form.find('button[type="submit"]');
        const originalText = button.html();

        // Show loading state
        button.prop('disabled', true).html('<span class="loading-spinner"></span> Processing...');

        $.ajax({
            url: form.attr('action'),
            method: form.attr('method') || 'POST',
            data: form.serialize(),
            success: function (response) {
                // Show success message
                showToast('Success', 'Operation completed successfully', 'success');
                
                // Reload page or update UI
                setTimeout(function () {
                    location.reload();
                }, 1000);
            },
            error: function (xhr) {
                // Show error message
                showToast('Error', 'An error occurred. Please try again.', 'error');
                button.prop('disabled', false).html(originalText);
            }
        });
    });

    // Toast notification function
    function showToast(title, message, type) {
        const bgColor = type === 'success' ? 'bg-success' : 'bg-danger';
        const icon = type === 'success' ? 'fas fa-check' : 'fas fa-exclamation-triangle';

        const toast = $(`
            <div class="toast ${bgColor}" role="alert" aria-live="assertive" aria-atomic="true" data-delay="3000">
                <div class="toast-header">
                    <i class="${icon} mr-2"></i>
                    <strong class="mr-auto">${title}</strong>
                    <button type="button" class="ml-2 mb-1 close" data-dismiss="toast" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="toast-body">
                    ${message}
                </div>
            </div>
        `);

        // Add to toast container or create one
        let container = $('.toast-container');
        if (container.length === 0) {
            container = $('<div class="toast-container" style="position: fixed; top: 20px; right: 20px; z-index: 9999;"></div>');
            $('body').append(container);
        }

        container.append(toast);
        toast.toast('show');

        // Remove after hiding
        toast.on('hidden.bs.toast', function () {
            $(this).remove();
        });
    }

    // Auto-hide alerts after 5 seconds
    $('.alert:not(.alert-permanent)').delay(5000).fadeOut('slow');

    // Sidebar toggle state persistence
    $('[data-widget="pushmenu"]').on('click', function () {
        const collapsed = $('body').hasClass('sidebar-collapse');
        localStorage.setItem('sidebar-collapsed', collapsed ? 'false' : 'true');
    });

    // Restore sidebar state
    const sidebarCollapsed = localStorage.getItem('sidebar-collapsed');
    if (sidebarCollapsed === 'true') {
        $('body').addClass('sidebar-collapse');
    }

    // Number counter animation for dashboard stats
    $('.counter').each(function () {
        const $this = $(this);
        const countTo = parseInt($this.text().replace(/,/g, ''));

        $({ countNum: 0 }).animate({
            countNum: countTo
        }, {
            duration: 1500,
            easing: 'swing',
            step: function () {
                $this.text(Math.floor(this.countNum).toLocaleString());
            },
            complete: function () {
                $this.text(countTo.toLocaleString());
            }
        });
    });

    // Bulk action checkbox handling
    $('#select-all').on('change', function () {
        $('.item-checkbox').prop('checked', $(this).prop('checked'));
        updateBulkActionButtons();
    });

    $('.item-checkbox').on('change', function () {
        updateBulkActionButtons();
    });

    function updateBulkActionButtons() {
        const checkedCount = $('.item-checkbox:checked').length;
        if (checkedCount > 0) {
            $('.bulk-action-btn').prop('disabled', false);
            $('.selected-count').text(checkedCount);
        } else {
            $('.bulk-action-btn').prop('disabled', true);
            $('#select-all').prop('checked', false);
        }
    }

    // Initialize tooltips
    if ($.fn.tooltip) {
        $('[data-toggle="tooltip"]').tooltip();
    }

    // Initialize popovers
    if ($.fn.popover) {
        $('[data-toggle="popover"]').popover();
    }
});
