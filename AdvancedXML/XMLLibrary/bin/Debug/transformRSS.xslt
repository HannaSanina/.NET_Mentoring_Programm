<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:x="http://library.by/catalog" exclude-result-prefixes="x">
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="/x:catalog">
    <rss version="2.0">
      <channel>
        <title>Books News</title>
        <link>http://my.safaribooksonline.com/</link>
        <description>Latest books</description>
        <language>en-us</language>
        <pubDate>Tue, 22 Now 2017 04:00:00 GMT</pubDate>
        <xsl:apply-templates />
      </channel>
    </rss>
  </xsl:template>

  <!--<xsl:template match="x:book[not(x:genre = &quot;Computer&quot;)]">
    <item>
      <title>
        <xsl:value-of select="x:title"/>
      </title>
      <description>
        <xsl:value-of select="x:description"/>
      </description>
      <pubDate>
        <xsl:value-of select="x:publish_date"/>
      </pubDate>

    </item>
  </xsl:template>-->

  <xsl:template match="x:book">
    <xsl:param name="isbnNumber" select="x:isbn"></xsl:param>
    <item>
      <title>
        <xsl:value-of select="x:title"/>
      </title>
      <xsl:choose>
        <xsl:when test="x:isbn and x:genre=&quot;Computer&quot;">
          <!--<xsl:choose>
            <xsl:when test="x:genre=&quot;Computer&quot;">-->
              <link>
                <xsl:call-template name="linkTemplate">
                  <xsl:with-param name="isbn">
                    <xsl:value-of select="x:isbn"/>
                  </xsl:with-param>
                </xsl:call-template>
              </link>
            <!--</xsl:when>
          </xsl:choose>-->
 
        </xsl:when>
      </xsl:choose>
      <!--<link>
        <xsl:call-template name="linkTemplate">
          <xsl:with-param name="isbn">
            <xsl:value-of select="x:isbn"/>
          </xsl:with-param>
        </xsl:call-template>
      </link>-->
      <description>
        <xsl:value-of select="x:description"/>
      </description>
      <pubDate>
        <xsl:value-of select="x:publish_date"/>
      </pubDate>
    </item>
  </xsl:template>
  
  <xsl:template name="linkTemplate">
    <xsl:param name="isbn" />
    <xsl:variable name="linktext" select="concat('http://my.safaribooksonline.com/', $isbn)"/>
    <xsl:value-of select="$linktext"/>
  </xsl:template>

  <xsl:template match="text() | @*"/>
</xsl:stylesheet>
