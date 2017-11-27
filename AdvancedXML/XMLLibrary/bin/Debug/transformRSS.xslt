<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:x="http://library.by/catalog"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:cs="urn:cs" exclude-result-prefixes="x msxsl cs">
  <xsl:output method="xml" indent="yes"/>
  <msxsl:script language="C#" implements-prefix="cs">
    <![CDATA[ 
        public string getTimeZone()
        {
            int offset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;
            string timeZone = "+" + offset.ToString().PadLeft(2, '0');
            if (offset < 0)
            {
                int i = offset * -1;
                timeZone = "-" + i.ToString().PadLeft(2, '0');
            }

            return timeZone;
        }
        public string datenow()
        {
            return (DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss " + getTimeZone().PadRight(5, '0')));
        }

        public string convertDate(string date)
        {
            var s = date.Split('-');
            DateTime d = new DateTime(Int32.Parse(s[0]), Int32.Parse(s[1]), Int32.Parse(s[2]));
            return (d.ToString("ddd, dd MMM yyyy HH:mm:ss ") + getTimeZone().PadRight(5, '0'));
        }]]>
  </msxsl:script>

  <xsl:template match="/x:catalog">
    <rss version="2.0">
      <channel>
        <title>Books News</title>
        <link>http://my.safaribooksonline.com/</link>
        <description>Latest books</description>
        <language>en-us</language>
        <pubDate>
          <xsl:value-of select="cs:datenow()"/>
        </pubDate>
        <xsl:apply-templates />
      </channel>
    </rss>
  </xsl:template>
  <xsl:template match="x:book">
    <xsl:param name="isbnNumber" select="x:isbn"></xsl:param>
    <item>
      <title>
        <xsl:value-of select="x:title"/>
      </title>
      <xsl:choose>
        <xsl:when test="x:isbn and x:genre=&quot;Computer&quot;">
          <link>
            <xsl:call-template name="linkTemplate">
              <xsl:with-param name="isbn">
                <xsl:value-of select="x:isbn"/>
              </xsl:with-param>
            </xsl:call-template>
          </link>
        </xsl:when>
      </xsl:choose>

      <description>
        <xsl:value-of select="x:description"/>
      </description>
      <pubDate>
        <xsl:call-template name="convertDate">
          <xsl:with-param name="date">
            <xsl:value-of select="x:publish_date"/>
          </xsl:with-param>
        </xsl:call-template>
      </pubDate>
    </item>
  </xsl:template>

  <xsl:template name="linkTemplate">
    <xsl:param name="isbn" />
    <xsl:variable name="linktext" select="concat('http://my.safaribooksonline.com/', $isbn)"/>
    <xsl:value-of select="$linktext"/>
  </xsl:template>
  
  <xsl:template name ="convertDate">
  <xsl:param name ="date" />
    <xsl:variable name="formattedDate" select="cs:convertDate($date)"/>
    <xsl:value-of select="$formattedDate"/>
  </xsl:template>

  <xsl:template match="text() | @*"/>
</xsl:stylesheet>
