# sqlmap ![](https://i.imgur.com/fe85aVR.png)

[![Build Status](https://api.travis-ci.org/sqlmapproject/sqlmap.svg?branch=master)](https://travis-ci.org/sqlmapproject/sqlmap) [![Python 2.6|2.7|3.x](https://img.shields.io/badge/python-2.6|2.7|3.x-yellow.svg)](https://www.python.org/) [![License](https://img.shields.io/badge/license-GPLv2-red.svg)](https://raw.githubusercontent.com/sqlmapproject/sqlmap/master/LICENSE) [![PyPI version](https://badge.fury.io/py/sqlmap.svg)](https://badge.fury.io/py/sqlmap) [![GitHub closed issues](https://img.shields.io/github/issues-closed-raw/sqlmapproject/sqlmap.svg?colorB=ff69b4)](https://github.com/sqlmapproject/sqlmap/issues?q=is%3Aissue+is%3Aclosed) [![Twitter](https://img.shields.io/badge/twitter-@sqlmap-blue.svg)](https://twitter.com/sqlmap)

sqlmap là một công cụ kiểm tra thâm nhập mã nguồn mở, nhằm tự động hóa quá trình phát hiện, khai thác lỗ hổng tiêm SQL và tiếp quản các máy chủ cơ sở dữ liệu. Nó đi kèm với 
một hệ thống phát hiện mạnh mẽ, nhiều tính năng thích hợp cho người kiểm tra thâm nhập và một loạt các tùy chọn bao gồm lấy dấu cơ sở dữ liệu, truy xuất dữ liệu từ cơ sở dữ 
liệu, truy cập tệp của hệ thống và thực hiện các lệnh trên hệ điều hành thông qua kết nối ngoài.

Ảnh chụp màn hình
----

![Screenshot](https://raw.github.com/wiki/sqlmapproject/sqlmap/images/sqlmap_screenshot.png)

Bạn có thể truy cập vào [bộ sưu tập ảnh chụp màn hình](https://github.com/sqlmapproject/sqlmap/wiki/Screenshots), chúng trình bày một số tính năng trên wiki.

Cài đặt
----


Bạn có thể tải xuống tập tin nén tar mới nhất bằng cách nhấp vào [đây](https://github.com/sqlmapproject/sqlmap/tarball/master) hoặc tập tin nén zip mới nhất bằng cách nhấp vào [đây](https://github.com/sqlmapproject/sqlmap/zipball/master).

Tốt hơn là bạn có thể tải xuống sqlmap bằng cách clone với [Git](https://github.com/sqlmapproject/sqlmap):

    git clone --depth 1 https://github.com/sqlmapproject/sqlmap.git sqlmap-dev

sqlmap hoạt động hiệu quả với [Python](http://www.python.org/download/) phiên bản **2.6**, **2.7** và **3.x** trên bất kì nền tảng nào.

Sử dụng
----

Để có được danh sách các tùy chọn cơ bản, hãy sử dụng:

    python sqlmap.py -h

Để có được danh sách tất cả các tùy chọn, hãy sử dụng:

    python sqlmap.py -hh

Bạn có thể tìm thấy video chạy mẫu [tại đây](https://asciinema.org/a/46601).
Để có cái nhìn tổng quan về các khả năng của sqlmap, danh sách các tính năng được hỗ trợ và mô tả về tất cả các tùy chọn, cùng với các ví dụ, bạn nên tham khảo [hướng dẫn sử dụng](https://github.com/sqlmapproject/sqlmap/wiki/Usage) (Tiếng Anh).

Liên kết
----

* Trang chủ: http://sqlmap.org
* Tải xuống: [.tar.gz](https://github.com/sqlmapproject/sqlmap/tarball/master) hoặc [.zip](https://github.com/sqlmapproject/sqlmap/zipball/master)
* Lịch sử thay nguồn đổi cấp dữ liệu RSS: https://github.com/sqlmapproject/sqlmap/commits/master.atom
* Theo dõi vấn đề: https://github.com/sqlmapproject/sqlmap/issues
* Hướng dẫn sử dụng: https://github.com/sqlmapproject/sqlmap/wiki
* Các câu hỏi thường gặp (FAQ): https://github.com/sqlmapproject/sqlmap/wiki/FAQ
* Twitter: [@sqlmap](https://twitter.com/sqlmap)
* Demo: [http://www.youtube.com/user/inquisb/videos](http://www.youtube.com/user/inquisb/videos)
* Ảnh chụp màn hình: https://github.com/sqlmapproject/sqlmap/wiki/Screenshots
