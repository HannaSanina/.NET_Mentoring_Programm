<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:x="http://library.by/catalog"
                version="1.0"
                exclude-result-prefixes="x msxsl"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:cs="urn:cs">

  <xsl:output method="html" indent="yes"/>
  <msxsl:script language="C#" implements-prefix="cs">
    <![CDATA[
      public string datenow()
     {
        return(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"));
     }
     ]]>
  </msxsl:script>

  <xsl:template match="/x:catalog">
    <div>
      <h2>CURRENT BOOKFOND</h2>
      <div>
        <xsl:value-of select="cs:datenow()"/>
      </div>

      <br></br>
      COMPUTER GENRE
      <table border="1">
        <tr>
          <th>Author</th>
          <th>Title</th>
          <th>Publish Date</th>
          <th>Registration Date</th>
        </tr>
        <xsl:variable name="list" select="x:book[x:genre = &quot;Computer&quot;]"></xsl:variable>
        <xsl:for-each select="$list">
          <tr>
            <td>
              <xsl:value-of select="x:author"/>
            </td>
            <td>
              <xsl:value-of select="x:title"/>
            </td>
            <td>
              <xsl:value-of select="x:publish_date"/>
            </td>
            <td>
              <xsl:value-of select="x:registration_date"/>
            </td>
          </tr>
        </xsl:for-each>
        <tr>
          <td>Total:</td>
          <td>
            <xsl:value-of select="count($list)"/>
          </td>
          <td></td>
          <td></td>
        </tr>
      </table>

      <br></br>
      FANTASY GENRE
      <table border="1">
        <tr>
          <th>Author</th>
          <th>Title</th>
          <th>Publish Date</th>
          <th>Registration Date</th>
        </tr>
        <xsl:variable name="list" select="x:book[x:genre = &quot;Fantasy&quot;]"></xsl:variable>
        <xsl:for-each select="$list">
          <tr>
            <td>
              <xsl:value-of select="x:author"/>
            </td>
            <td>
              <xsl:value-of select="x:title"/>
            </td>
            <td>
              <xsl:value-of select="x:publish_date"/>
            </td>
            <td>
              <xsl:value-of select="x:registration_date"/>
            </td>
          </tr>
        </xsl:for-each>
        <tr>
          <td>Total:</td>
          <td>
            <xsl:value-of select="count($list)"/>
          </td>
          <td></td>
          <td></td>
        </tr>
      </table>

      <br></br>
      ROMANCE GENRE
      <table border="1">
        <tr>
          <th>Author</th>
          <th>Title</th>
          <th>Publish Date</th>
          <th>Registration Date</th>
        </tr>
        <xsl:variable name="list" select="x:book[x:genre = &quot;Romance&quot;]"></xsl:variable>
        <xsl:for-each select="$list">
          <tr>
            <td>
              <xsl:value-of select="x:author"/>
            </td>
            <td>
              <xsl:value-of select="x:title"/>
            </td>
            <td>
              <xsl:value-of select="x:publish_date"/>
            </td>
            <td>
              <xsl:value-of select="x:registration_date"/>
            </td>
          </tr>
        </xsl:for-each>
        <tr>
          <td>Total:</td>
          <td>
            <xsl:value-of select="count($list)"/>
          </td>
          <td></td>
          <td></td>
        </tr>
      </table>

      <br></br>
      HORROR GENRE
      <table border="1">
        <tr>
          <th>Author</th>
          <th>Title</th>
          <th>Publish Date</th>
          <th>Registration Date</th>
        </tr>
        <xsl:variable name="list" select="x:book[x:genre = &quot;Horror&quot;]"></xsl:variable>
        <xsl:for-each select="$list">
          <tr>
            <td>
              <xsl:value-of select="x:author"/>
            </td>
            <td>
              <xsl:value-of select="x:title"/>
            </td>
            <td>
              <xsl:value-of select="x:publish_date"/>
            </td>
            <td>
              <xsl:value-of select="x:registration_date"/>
            </td>
          </tr>
        </xsl:for-each>
        <tr>
          <td>Total:</td>
          <td>
            <xsl:value-of select="count($list)"/>
          </td>
          <td></td>
          <td></td>
        </tr>
      </table>
      <div>
        Total books count: <xsl:value-of disable-output-escaping="yes" select="count(x:book)"/>
      </div>
    </div>

  </xsl:template>
</xsl:stylesheet>
