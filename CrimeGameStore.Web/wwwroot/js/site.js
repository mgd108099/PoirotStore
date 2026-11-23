document.addEventListener("DOMContentLoaded", function () {
    // 1. تفعيل الرابط الحالي في شريط التنقل (Navbar Active State)
    const currentPath = window.location.pathname.toLowerCase();
    const navLinks = document.querySelectorAll(".poirot-navbar .nav-link");

    navLinks.forEach(link => {
        const href = link.getAttribute("href")?.toLowerCase();
        if (href === currentPath || (currentPath.startsWith("/cases") && href === "/cases")) {
            link.classList.add("active");
        }
    });

    // 2. جلب عدد العناصر في حقيبة القضايا وتحديث العداد في الهيدر
    const cartBadge = document.getElementById("cartCountBadge");
    if (cartBadge) {
        fetch("/cart/count")
            .then(res => res.ok ? res.json() : null)
            .then(data => {
                if (data && typeof data.count === "number") {
                    cartBadge.textContent = data.count;
                }
            })
            .catch(() => { /* تجاهل بهدوء في حال انقطاع الاتصال */ });
    }
});