# OJStatusCrawler

基于.net core的爬虫，主要用于爬取大佬的提交记录，用于了解大佬的做题顺序。

目前已支持抓取HDU,POJ与BZOJ的AC记录

## 使用方法

```
命令行使用方法:
user:   指定用户名,若不指定,则从stdin读入
oj:     指定OJ,若不指定,则从stdin读入
            目前支持bzoj,hdu,poj
output: 指定输出文件(默认result.md),附加到文件末尾
mode:   firstac(默认),ac,all(尚未支持)
reverse:是否倒序输出(默认true)

示例:
    dotnet run user=llf0703 oj=bzoj
        抓取llf0703的bzoj上的AC提交记录.
```

## 建议了解

[llf0703/pld](https://github.com/Llf0703/pld)
